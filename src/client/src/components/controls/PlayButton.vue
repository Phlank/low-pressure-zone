<template>
  <Button
    :icon="controlIcon"
    @click="handleControlClick"
    :label="controlLabel"
    size="large"
    rounded
  ></Button>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'
import { PlayState, toggle } from '@/constants/playState'
import { computed, ref, type Ref } from 'vue'

const djName: Ref<string> = ref('strgrll')
const streamType: Ref<string> = ref('Live DJ Set')
const controlState: Ref<PlayState> = ref(PlayState.Paused)

const handleControlClick = () => {
  toggle(controlState)
}

const controlIcon = computed(() => {
  if (controlState.value == PlayState.Paused) {
    return 'pi pi-play'
  }
  return 'pi pi-pause'
})

const controlLabel = computed(() => `${djName.value} | ${streamType.value}`)
</script>

<style lang="scss" scoped>
.control-icon {
  font-size: 1rem;
}
</style>
