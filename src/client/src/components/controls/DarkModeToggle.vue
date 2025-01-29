<template>
  <div class="dark-mode-toggle">
    <i class="pi pi-sun"></i>
    <ToggleSwitch
      :model-value="isDarkMode"
      @value-change="handleDarkModeToggle"
      class="dark-mode-toggle"
    ></ToggleSwitch>
    <i class="pi pi-moon"></i>
  </div>
</template>

<script lang="ts" setup>
import { ToggleSwitch } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'

const isDarkMode: Ref<boolean> = ref(true)
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

const handleDarkModeToggle = (newValue: boolean) => {
  isDarkMode.value = newValue
  document.documentElement.classList.toggle('dark-mode-toggle')
  document.body.classList.toggle('dark-mode-toggle')
}
</script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

.dark-mode-toggle {
  display: flex;
  align-items: center;

  i {
    margin: variables.$space-m;
  }
}
</style>
