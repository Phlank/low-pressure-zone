<template>
  <ContentPanel class="footer">
    <div class="left">
      <div class="left__controls" @click="handleControlClick">
        <span class="material-symbols-outlined">{{ controlIcon }}</span>
      </div>
      <div class="left__stream-metadata">
        <span>{{ djName }}</span>
        <span class="material-symbols-outlined">chevron_right</span>
        <span>{{ streamType }}</span>
      </div>
    </div>
    <div class="right" v-if="!isMobile">
      <div class="right__image-metadata">Image source: {{ imageSource }}</div>
    </div>
  </ContentPanel>
</template>

<script lang="ts" setup>
import { PlayState, toggle } from '@/constants/playState'
import ContentPanel from '@/components/shared/ContentPanel.vue'
import { computed, inject, ref, type Ref } from 'vue'

const isMobile = inject('isMobile')
const djName: Ref<string> = ref('Phlank')
const streamType: Ref<string> = ref('Live DJ Set')
const imageSource: Ref<string> = ref('NOAA')
const controlState: Ref<PlayState> = ref(PlayState.Paused)

const handleControlClick = () => {
  toggle(controlState)
}

const controlIcon = computed(() => {
  if (controlState.value == PlayState.Paused) {
    return 'play_circle'
  }
  return 'pause_circle'
})
</script>

<style lang="scss" scoped>
@import './../../assets/variables.scss';

$footer-height: 48px;

.footer {
  display: flex;
  justify-content: center;
  height: $footer-height;
}

.left {
  display: flex;
  float: left;
  margin-right: auto;

  &__controls {
    display: inline-block;
    color: $font-color-white-darker;

    .material-symbols-outlined {
      font-size: $footer-height;
      color: $control-color;
    }
    :hover {
      cursor: pointer;
      color: $control-color-hover;
    }
  }

  &__stream-metadata {
    padding-left: $space-m;
    font-size: larger;
    display: flex;
    margin: auto 0 auto 0;
  }
}

.right {
  float: right;
  margin: auto 0 auto auto;

  &__image-metadata {
    font-size: larger;
    color: $font-color-white-darker;
    padding-left: $space-m;
  }
}
</style>
