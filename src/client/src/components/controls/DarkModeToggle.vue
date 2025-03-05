<template>
  <div class="dark-mode-toggle">
    <i class="pi pi-sun"></i>
    <ToggleSwitch
      :model-value="isDarkMode"
      @value-change="handleDarkModeToggle"
      class="dark-mode-toggle" />
    <i class="pi pi-moon"></i>
  </div>
</template>

<script lang="ts" setup>
import { isTrueString } from '@/utils/booleanUtils'
import { useLocalStorage } from '@vueuse/core'
import { ToggleSwitch } from 'primevue'
import { computed, onMounted } from 'vue'

const isDarkModeStored = useLocalStorage('isDarkMode', 'true')
const toggleDarkModeStored = () => {
  if (isTrueString(isDarkModeStored.value)) {
    isDarkModeStored.value = 'false'
  } else {
    isDarkModeStored.value = 'true'
  }
}
const isDarkMode = computed(() => isTrueString(isDarkModeStored.value))

onMounted(() => {
  if (
    isDarkMode.value &&
    !document.documentElement.classList.contains('dark-mode-toggle') &&
    !document.body.classList.contains('dark-mode-toggle')
  ) {
    document.documentElement.classList.toggle('dark-mode-toggle')
    document.body.classList.toggle('dark-mode-toggle')
  } else if (!isDarkMode.value && document.documentElement.classList.contains('dark-mode-toggle')) {
    document.documentElement.classList.toggle('dark-mode-toggle')
    document.body.classList.toggle('dark-mode-toggle')
  }
})

const handleDarkModeToggle = () => {
  toggleDarkModeStored()
  document.documentElement.classList.toggle('dark-mode-toggle')
  document.body.classList.toggle('dark-mode-toggle')
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.dark-mode-toggle {
  display: flex;
  align-items: center;

  i {
    margin: variables.$space-m;
  }
}
</style>
