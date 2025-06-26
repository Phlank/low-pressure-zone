<template>
  <div class="play-button">
    <div class="play-button__buttons">
      <Button
        ref="buttonElement"
        :disabled="!isPlayable"
        class="play-button__play-element"
        rounded
        size="large"
        @click="togglePlaying">
        <div class="play-button__content">
          <div class="play-button__content__icon">
            <span :class="controlIcon"></span>
          </div>
          <div class="play-button__content__text-area">
            <div class="play-button__content__text-area__status">
              {{ loadingText }} | {{ listenerText }}
            </div>
            <div class="play-button__content__text-area__now-playing">
              <div class="play-button__content__text-area__now-playing__text">
                {{ streamStatus?.name }}
              </div>
            </div>
          </div>
        </div>
      </Button>
      <Button
        class="play-button__volume-element"
        rounded
        size="large"
        @click="toggleVolumeSlider">
        <span class="pi pi-volume-up" />
      </Button>
    </div>
    <div class="play-button__volume">
      <Slider
        v-model.number="volumeSliderAmount"
        class="play-button__volume__slider" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Button, Slider, useToast } from 'primevue'
import { computed, type ComputedRef, onMounted, type Ref, ref, watch } from 'vue'
import delay from '@/utils/delay.ts'
import streamApi, { type StreamStatusResponse } from '@/api/resources/streamApi.ts'
import clamp from '@/utils/clamp.ts'
import { useResizeObserver } from '@vueuse/core'

const toast = useToast()

enum PlayState {
  Playing,
  Paused
}

const disconnectedWaitText = 'Disconnected. Attempting to reconnect...'
const nobodyDjText = 'Nobody'
const streamName: Ref<string> = ref(nobodyDjText)
const playState: Ref<PlayState> = ref(PlayState.Paused)
const isLoading: Ref<boolean> = ref(false)
const streamStatus: Ref<StreamStatusResponse | undefined> = ref(undefined)
let audio: HTMLAudioElement | undefined = undefined
let audioAbortController = new AbortController()

const isPlayable: ComputedRef<boolean> = computed(() => streamName.value !== nobodyDjText)

const loadingText = computed(() => {
  if (streamStatus.value?.isLive === undefined) return 'Loading...'
  if (streamStatus.value?.isLive) return 'Live'
  return 'Offline'
})

const listenerText = computed(() =>
  streamStatus.value ? 'Listeners: ' + streamStatus.value.listenerCount : ''
)

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
  waitForReconnect()
}

const waitForReconnect = () => {
  streamName.value = disconnectedWaitText
  stopAudio()
}

const setNobodyPlaying = () => {
  streamName.value = nobodyDjText
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
  waitForReconnect()
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
        updateStatus(response?.data())
      }
    } catch (error: unknown) {
      console.log(JSON.stringify(error))
    } finally {
      await delay(5000)
    }
  }
}

const updateStatus = (newStatus: StreamStatusResponse) => {
  if (
    newStatus.isLive !== streamStatus.value?.isLive ||
    newStatus.isOnline !== streamStatus.value?.isOnline ||
    (newStatus.name ?? 'Unknown') !== streamStatus.value?.name ||
    newStatus.listenUrl !== streamStatus.value?.listenUrl ||
    newStatus.isStatusStale !== streamStatus.value?.isStatusStale ||
    newStatus.type !== streamStatus.value?.type ||
    newStatus.listenerCount !== streamStatus.value?.listenerCount
  ) {
    streamStatus.value = newStatus
    if (!newStatus.isOnline) {
      newStatus.name = disconnectedWaitText
    }
    streamName.value = streamStatus.value?.name ?? 'Unknown'
    setTimeout(() => updateTextScrollingBehavior(), 100)
  }
}

const controlIcon = computed(() => {
  if (playState.value === PlayState.Paused) {
    return 'pi pi-play-circle'
  }
  return 'pi pi-pause-circle'
})

const textWidth = ref(0)
const textWidthPx = computed(() => Math.round(textWidth.value) + 'px')
const buttonWidth = ref(0)
const buttonPadding = 30
const playIconWidth = 28
const centerMargin = 10
const textTranslateWidth = ref(0)
const textTranslateWidthPx = computed(() => Math.round(textTranslateWidth.value) + 'px')
const textScrollAnimationDuration = computed(() => (4 * -textTranslateWidth.value) / 50 + 's')

const buttonElement = ref(null)
useResizeObserver(buttonElement, () => updateTextScrollingBehavior())

const updateTextScrollingBehavior = () => {
  textWidth.value = document
    .getElementsByClassName('play-button__content__text-area__now-playing')[0]
    .getBoundingClientRect().width
  buttonWidth.value = document
    .getElementsByClassName('play-button__play-element')[0]
    .getBoundingClientRect().width
  let translateWidth = Math.round(
    clamp(textWidth.value - buttonWidth.value + buttonPadding + playIconWidth + centerMargin, 0)
  )
  if (Math.abs(translateWidth) < 5) {
    translateWidth = 0
  } else {
    translateWidth = -translateWidth
  }
  textTranslateWidth.value = translateWidth
}

const volumeSliderAmount = ref(100)
const volume = computed(() => volumeSliderAmount.value / 100)
const showVolumeSliderArea = ref(false)
const showVolumeSlider = ref(false)
const volumeSliderDisplayValue = computed(() => (showVolumeSlider.value ? 'block' : 'none'))
const volumeSliderAreaPaddingValue = computed(() => (showVolumeSliderArea.value ? '20px' : '0px'))
const volumeSliderAreaMarginTopValue = computed(() => (showVolumeSliderArea.value ? '10px' : '0px'))
const volumeSliderAreaBorder = computed(() =>
  showVolumeSliderArea.value ? '1px solid var(--p-button-primary-border-color)' : 'none'
)

watch(volume, () => {
  if (audio !== undefined) {
    audio.volume = volume.value
  }
})

const toggleVolumeSlider = () => {
  const isDisplayed = showVolumeSliderArea.value
  showVolumeSliderArea.value = !isDisplayed
  setTimeout(() => {
    showVolumeSlider.value = !isDisplayed
  }, 100)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

$text-width: v-bind(textWidthPx);
$text-translate-amount: v-bind(textTranslateWidthPx);

.play-button {
  width: min(
    400px,
    calc(100dvw - 2 * #{variables.$space-l}),
    calc(30px + 28px + 10px + 46px + 10px + #{$text-width})
  );

  &__buttons {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: variables.$space-m;
  }

  &__play-element {
    width: 100%;
    height: 100%;
    z-index: 10;
  }

  &__volume-element {
    height: 46px;

    .pi {
      font-size: 1.25rem;
    }
  }

  &__content {
    overflow: hidden;
    display: flex;

    &__icon {
      margin: auto 0;

      .pi {
        font-size: 1.75rem;
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
          animation-duration: v-bind(textScrollAnimationDuration);
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

  &__volume {
    width: 100%;
    z-index: 5;
    height: 0;
    background-color: var(--p-surface-800);
    padding: v-bind(volumeSliderAreaPaddingValue);
    margin-top: v-bind(volumeSliderAreaMarginTopValue);
    transition:
      padding 0.1s ease-in-out,
      margin-top 0.1s ease-in-out,
      border 0.1s ease-in-out;
    border-radius: calc(2 * #{variables.$space-l});
    border: v-bind(volumeSliderAreaBorder);

    div.p-slider {
      display: v-bind(volumeSliderDisplayValue);
    }
  }
}
</style>
