<template>
  <Button
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

const toast = useToast()

enum PlayState {
  Playing,
  Paused
}

const djName: Ref<string> = ref('strgrll')
const streamType: Ref<string> = ref('Live DJ Set')
const playState: Ref<PlayState> = ref(PlayState.Paused)
const isLoading: Ref<boolean> = ref(false)
let audio: HTMLAudioElement | undefined = undefined
let audioAbortController = new AbortController()

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
  if (newPlayState === PlayState.Paused) {
    audio?.pause()
    return
  }

  // Create the HTMLAudioElement and begin loading data; start playing when enough is available
  audioAbortController = new AbortController()
  if (audio === undefined) {
    audio = new Audio(import.meta.env.VITE_LISTEN_URL)
    audio.preload = 'metadata'
    addAudioEventListeners()
    isLoading.value = true
  } else {
    if (audio) audio.currentTime = audio.duration - 1
    audio.play()
  }
})

const addAudioEventListeners = () => {
  const listenerOptions = { signal: audioAbortController.signal }
  audio?.addEventListener('canplay', handleCanPlay, listenerOptions)
  audio?.addEventListener('ended', handleEnded, listenerOptions)
  audio?.addEventListener('waiting', handleWaiting, listenerOptions)
  audio?.addEventListener('error', handleError, listenerOptions)
}

const handleCanPlay = () => {
  if (playState.value === PlayState.Playing) {
    audio?.play()
    isLoading.value = false
  }
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
    const json = await result.json()
    console.log(JSON.stringify(json))
    if (json.icestats?.source?.server_name !== undefined) {
      djName.value = json.icestats.source.server_name
      streamType.value = 'Live DJ Set'
    } else {
      djName.value = 'Nobody'
      streamType.value = 'Nothing'
    }
    await delay(10000)
  }
}

const controlIcon = computed(() => {
  if (playState.value === PlayState.Paused) {
    return 'pi pi-play'
  }
  return 'pi pi-pause'
})

const controlLabel = computed(() => `${djName.value} | ${streamType.value}`)
</script>

<style lang="scss">
.play-button {
  width: fit-content;
}
</style>
