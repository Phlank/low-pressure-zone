<template>
  <Button
    :disabled="!isPlayable"
    class="play-button"
    rounded
    size="large"
    @click="togglePlaying">
    <div class="play-button__content">
      <div class="play-button__content__icon">
        <span :class="controlIcon"></span>
      </div>
      <div class="play-button__content__text-area">
        <div class="play-button__content__text-area__status">{{ loadingText }}</div>
        <div class="play-button__content__text-area__now-playing">{{ streamStatus?.name }}</div>
      </div>
    </div>
  </Button>
</template>

<script lang="ts" setup>
import { Button, useToast } from 'primevue'
import { computed, type ComputedRef, onMounted, type Ref, ref, watch } from 'vue'
import delay from '@/utils/delay.ts'
import { noStatsToast } from '@/constants/toasts.ts'
import streamApi, { type StreamStatusResponse } from '@/api/resources/streamApi.ts'

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
const streamStatus: Ref<StreamStatusResponse | undefined> = ref(undefined)
let audio: HTMLAudioElement | undefined = undefined
let audioAbortController = new AbortController()

const isPlayable: ComputedRef<boolean> = computed(() => djName.value !== nobodyDjText)

const loadingText = computed(() => {
  if (streamStatus.value?.isLive === undefined) return 'Loading...'
  if (streamStatus.value?.isLive) return 'Live'
  return 'Offline'
})

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
  audio = new Audio(streamStatus.value?.listenUrl ?? '')
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
      const response = await streamApi.getStatus()
      if (response.isSuccess()) {
        streamStatus.value = response.data()
      }
    } catch (error: unknown) {
      toast.add(noStatsToast)
      console.log(JSON.stringify(error))
    } finally {
      await delay(5000)
    }
  }
}

watch(streamStatus, (newStatus, oldStatus) => {
  if (newStatus === undefined) return
  if (newStatus.isOnline) {
    if (djName.value === nextDjWaitText) {
      startAudio()
    }
    djName.value = newStatus.name ?? 'Unknown'
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
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.play-button {
  max-width: min(350px, calc(100dvh - 2 * #{variables.$space-l}));

  &__content {
    overflow: hidden;
    display: flex;
    width: 100%;

    &__icon {
      margin: auto 0;

      .pi {
        font-size: large;
        vertical-align: middle;
      }
    }

    &__text-area {
      display: flex;
      flex-direction: column;
      align-items: start;
      margin-left: 10px;
      width: 100%;

      &__status {
        font-size: small;
      }

      &__now-playing {
        font-size: medium;
        text-align: left;
        overflow: hidden;
        white-space: nowrap;
      }
    }
  }
}
</style>
