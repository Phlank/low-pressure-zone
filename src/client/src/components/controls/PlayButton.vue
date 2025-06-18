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
        <div class="play-button__content__text-area__now-playing">
          <div class="play-button__content__text-area__now-playing__text">
            {{ streamStatus?.name }}
          </div>
        </div>
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
      try {
        audio.src = '' // This will always fail (on purpose), so catch it and ignore it.
      } catch (e) {
        console.log(e)
      }
      audio = undefined
    }
  } catch (error) {
    console.log(JSON.stringify(error))
  }
}

const startAudio = () => {
  isLoading.value = true
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
    return 'pi pi-play-circle'
  }
  return 'pi pi-pause-circle'
})

const textWidth = computed(() => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const updateHook = streamStatus.value?.name ?? '' // Force width update on status change
  return (
    document
      .getElementsByClassName('play-button__content__text-area__now-playing')[0]
      .getBoundingClientRect().width + 'px'
  )
})

const textTranslateAmount = computed(() => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const updateHook = streamStatus.value?.name ?? '' // Force width update on status change
  const translateAmount =
    -(
      document
        .getElementsByClassName('play-button__content__text-area__now-playing')[0]
        .getBoundingClientRect().width -
      350 +
      32 +
      30 +
      10
    ) +
    'px' +
    ' 0'
  console.log(translateAmount)
  return translateAmount
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

$text-width: v-bind(textWidth);
$text-translate-amount: v-bind(textTranslateAmount);

.play-button {
  width: min(
    350px,
    calc(100dvw - 2 * #{variables.$space-l}),
    calc(30px + 32px + 10px + #{$text-width})
  );

  &__content {
    overflow: hidden;
    display: flex;

    &__icon {
      margin: auto 0;

      .pi {
        font-size: 2rem;
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

        &__text {
          animation-name: slide;
          animation-duration: 20s;
          animation-timing-function: linear;
          animation-iteration-count: infinite;

          @keyframes slide {
            0% {
              translate: 0 0;
            }
            25% {
              translate: 0 0;
            }
            50% {
              translate: $text-translate-amount;
            }
            75% {
              translate: $text-translate-amount;
            }
            100% {
              translate: 0 0;
            }
          }
        }
      }
    }
  }
}
</style>
