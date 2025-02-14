<template>
  <Button
    class="play-button"
    :icon="controlIcon"
    @click="handleControlClick"
    :label="controlLabel"
    size="large"
    rounded
  ></Button>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'
import { computed, ref, type Ref } from 'vue'

enum PlayState {
  Playing = 'playing',
  Paused = 'paused'
}

const djName: Ref<string> = ref('strgrll')
const streamType: Ref<string> = ref('Live DJ Set')
const playState: Ref<PlayState> = ref(PlayState.Paused)

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

const controlIcon = computed(() => {
  if (playState.value == PlayState.Paused) {
    return 'pi pi-play'
  }
  return 'pi pi-pause'
})

const controlLabel = computed(() => `${djName.value} | ${streamType.value}`)
</script>

<style lang="scss" scoped>
.play-button {
  width: fit-content;
}
</style>
