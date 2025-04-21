<template>
  <Button
    :disabled="!isPlayable"
    :icon="controlIcon"
    :label="controlLabel"
    class="play-button"
    rounded
    size="large"
    @click="handleControlClick">
    <template #icon>
      <ProgressSpinner
        v-if="isLoading"
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
import { computed, onMounted, type Ref, ref, watch } from 'vue'
import delay from '@/utils/delay.ts'
import { noStatsToast } from '@/constants/toasts.ts'
import { getLiveSource, type IcecastStats } from '@/types/icecastStats.ts'

const toast = useToast()

enum PlayState {
  Playing,
  Paused
}

const djName: Ref<string> = ref('Nobody')
const streamType: Ref<string> = ref('Nothing')
const playState: Ref<PlayState> = ref(PlayState.Paused)
const isLoading: Ref<boolean> = ref(false)
const isPlayable: Ref<boolean> = ref(false)
let audio: HTMLAudioElement | undefined = undefined
let audioAbortController = new AbortController()

window.addEventListener('beforeunload', () => {
  audioAbortController.abort()
  audio = undefined
})

const handleControlClick = () => {
  toggle(playState)
}

const toggle = (ref: Ref<PlayState>) => {
  if (ref.value === PlayState.Paused) {
    ref.value = PlayState.Playing
  } else {
    ref.value = PlayState.Paused
  }
}

watch(playState, (newPlayState) => {
  if (newPlayState === PlayState.Paused && audio !== undefined) {
    audio.pause()
    return
  }

  let isNewAudio = false
  if (audio === undefined) {
    isNewAudio = true
    audioAbortController = new AbortController()
    audio = new Audio(import.meta.env.VITE_LISTEN_URL)
    audio.preload = 'metadata'
    addAudioEventListeners()
  }
  isLoading.value = true
  if (!isNaN(audio.duration) && !isNewAudio) {
    audio.fastSeek(audio.duration)
  }
  audio.play()
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
  }
}

const handlePlay = () => {
  isLoading.value = false
}

const handleEnded = () => {
  isLoading.value = false
  audio?.pause()
  audio = undefined
  audioAbortController.abort()
  // This has to be the last statement of the event listener because it will trigger the watch event
  // that removes all event listeners and set `audio` to undefined
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
    severity: 'error'
  })
  handleEnded()
}

onMounted(() => {
  pollStreamMetadata().then(() => {})
})

const pollStreamMetadata = async () => {
  while (true) {
    const result = await fetch(import.meta.env.VITE_STREAM_STATUS_URL)
    if (!result.ok) {
      toast.add(noStatsToast)
      isPlayable.value = false
      return
    }
    const json = await result.json()
    if (json.icestats === undefined) {
      toast.add(noStatsToast)
      isPlayable.value = false
      return
    }
    console.log(JSON.stringify(json))
    const stats = json.icestats as IcecastStats
    const liveSource = getLiveSource(stats)
    if (liveSource === undefined) {
      isPlayable.value = false
      djName.value = 'Nobody'
      return
    }
    djName.value = liveSource.artist ?? liveSource.title ?? liveSource.server_name ?? 'Unknown'
    streamType.value = 'Live DJ Set'
    isPlayable.value = true
    await delay(10000)
  }
}

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
