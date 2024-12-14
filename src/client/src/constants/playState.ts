import type { Ref } from 'vue'

export enum PlayState {
  Playing = 'playing',
  Paused = 'paused',
}

export const toggle = (ref: Ref<PlayState>) => {
  if (ref.value == PlayState.Paused) {
    ref.value = PlayState.Playing
  } else {
    ref.value = PlayState.Paused
  }
}
