<template>
  <div
    ref="appRef"
    class="app">
    <Toast class="app__toast" />
    <SiteLayout />
  </div>
</template>

<script lang="ts" setup>
import { Toast } from 'primevue'
import { computed, onMounted, provide, ref, type Ref } from 'vue'
import SiteLayout from './components/layout/site/SiteLayout.vue'
import { useLocalStorage, useResizeObserver } from '@vueuse/core'
import { mobileWidth } from '@/constants/size.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import { isTrueString } from '@/utils/booleanUtils.ts'

const authStore = useAuthStore()

onMounted(async () => {
  await authStore.loadIfNotInitialized()
})

const isMobile: Ref<boolean> = ref(false)
useResizeObserver(document.body, () => {
  isMobile.value = document.body.offsetWidth <= mobileWidth
})
provide('isMobile', isMobile)

const isDarkModeStored = useLocalStorage('isDarkMode', 'true')
const isDarkMode = computed({
  get() {
    return isTrueString(isDarkModeStored.value)
  },
  set(value) {
    if (value) {
      isDarkModeStored.value = 'true'
    } else {
      isDarkModeStored.value = 'false'
    }
  }
})
provide('isDarkMode', isDarkMode)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.app {
  min-width: 100vw;

  &__toast {
    max-width: calc(100vw - 2 * #{variables.$space-l});
  }
}
</style>
