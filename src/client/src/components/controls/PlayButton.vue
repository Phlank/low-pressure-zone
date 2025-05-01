<template>
  <Button
    :disabled="!isPlayable"
    :icon="controlIcon"
    :label="controlLabel"
    class="play-button"
    rounded
    size="large"
    @click="togglePlaying">
    <template #icon>
      <ProgressSpinner
        v-if="isLoading || isWaitingForNextDj"
        animationDuration="1s"
        aria-label="Custom ProgressSpinner"
        fill="transparent"
        strokeWidth="6"
        style="width: 18px; height: 18px" />
    </template>
  </Button>
</template>

<script lang="ts" setup>
import { Button, ProgressSpinner, useToast } from 'primevue'
import { computed, type ComputedRef, onMounted, type Ref, ref, watch } from 'vue'
import delay from '@/utils/delay.ts'
import { noStatsToast } from '@/constants/toasts.ts'
import icecastApi, { type IcecastStatusResponse } from '@/api/resources/icecastApi.ts'

const toast = useToast()

enum PlayState {
  Playing,
  Paused
}

const nextDjWaitText = 'Waiting for next DJ...'
const nobodyDjText = 'Nobody'
const djName: Ref<string> = ref(nobodyDjText)
const playState: Ref<PlayState> = ref(PlayState.Paused)
const isLoading: Ref<boolean> = ref(false)
const isWaitingForNextDj: Ref<boolean> = ref(false)
const icecastStatus: Ref<IcecastStatusResponse | undefined> = ref(undefined)
let audio: HTMLAudioElement | undefined = undefined
let audioAbortController = new AbortController()

const isPlayable: ComputedRef<boolean> = computed(() => djName.value !== nobodyDjText)

window.addEventListener('beforeunload', () => {
  if (audio !== undefined) {
    stopAudio()
  }
})

const stopAudio = () => {
  try {
    if (audio !== undefined) {
      audio.pause()
      audioAbortController.abort()
      audio.src = ''
      audio = undefined
    }
  } catch (error) {
    console.log(error)
  }
}

const startAudio = () => {
  isLoading.value = true
  console.log('startAudio: isLoading.value = true')
  audioAbortController = new AbortController()
  audio = new Audio(import.meta.env.VITE_LISTEN_URL)
  audio.preload = 'metadata'
  addAudioEventListeners()
  audio.play()
}

const togglePlaying = () => {
  if (playState.value === PlayState.Paused) {
    playState.value = PlayState.Playing
  } else {
    playState.value = PlayState.Paused
  }
}

watch(playState, (newPlayState) => {
  if (newPlayState === PlayState.Paused && audio !== undefined) {
    // Stream is playing and the user pressed pause
    stopAudio()
  } else if (newPlayState === PlayState.Paused && audio === undefined) {
    // Stream is waiting for next DJ and user pressed pause
    setNobodyPlaying()
  } else if (newPlayState === PlayState.Playing && audio === undefined) {
    // Stream is not playing and user presses play
    startAudio()
  }
})

const addAudioEventListeners = () => {
  const listenerOptions = { signal: audioAbortController.signal }
  audio?.addEventListener('canplay', handleCanPlay, listenerOptions)
  audio?.addEventListener('play', handlePlay, listenerOptions)
  audio?.addEventListener('ended', handleEnded, listenerOptions)
  audio?.addEventListener('waiting', handleWaiting, listenerOptions)
  audio?.addEventListener('error', handleError, listenerOptions)
}

const handleCanPlay = () => {
  if (playState.value === PlayState.Playing) {
    isLoading.value = false
    console.log('startAudio: isLoading.value = true')
  }
}

const handlePlay = () => {
  isLoading.value = false
}

const handleEnded = () => {
  waitForNextDj()
}

const waitForNextDj = () => {
  djName.value = nextDjWaitText
  stopAudio()
}

const setNobodyPlaying = () => {
  djName.value = nobodyDjText
  isLoading.value = false
  playState.value = PlayState.Paused
}

const handleWaiting = () => {
  isLoading.value = true
}

const handleError = () => {
  if (audio?.src === '') return
  toast.add({
    summary: 'Playback error',
    detail: 'Unable to load audio stream',
    severity: 'error',
    life: 7000
  })
  waitForNextDj()
}

onMounted(() => {
  pollStreamMetadata().then(() => {})
})

const pollStreamMetadata = async () => {
  // noinspection InfiniteLoopJS
  while (true) {
    try {
      const response = await icecastApi.getStatus()
      if (response.isSuccess()) {
        icecastStatus.value = response.data()
      }
    } catch (error: unknown) {
      toast.add(noStatsToast)
      console.log(JSON.stringify(error))
    } finally {
      await delay(10000)
    }
  }
}

watch(icecastStatus, (newStatus, oldStatus) => {
  if (newStatus === undefined) return
  if (newStatus.isLive) {
    if (djName.value === nextDjWaitText) {
      startAudio()
    }
    djName.value = newStatus.name ?? 'Unspecified'
    return
  }

  if (newStatus.isOnline) {
    if (playState.value === PlayState.Playing) {
      waitForNextDj()
    } else {
      setNobodyPlaying()
    }
    return
  }

  if (newStatus.isStatusStale && (oldStatus === undefined || !oldStatus.isStatusStale)) {
    toast.add({
      summary: 'Issue loading stream information',
      detail: 'No new stream information has been retrieved in 30s. Check the stream status page.',
      severity: 'warning',
      life: 7000
    })
  }
})

const controlIcon = computed(() => {
  if (playState.value === PlayState.Paused) {
    return 'pi pi-play'
  }
  return 'pi pi-pause'
})

const controlLabel = computed(() => `${djName.value}`)
</script>

<style lang="scss">
.play-button {
  width: fit-content;
}
</style>
