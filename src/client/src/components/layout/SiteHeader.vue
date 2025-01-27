<template>
  <Toolbar class="header">
    <template #start>Low Pressure Zone</template>
    <template #end>
      <DarkModeToggle />
      <div v-if="isMobile">
        <Button icon="pi pi-bars" size="large" outlined></Button>
      </div>
      <div v-else>
        <Button label="Schedules" outlined />
        <Button label="Chat" outlined />
      </div>
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Button, Toolbar } from 'primevue'
import { inject } from 'vue'
import DarkModeToggle from '../controls/DarkModeToggle.vue'

const isMobile = inject<boolean>('isMobile')
</script>

<style lang="scss" scoped>
@use './../../assets/main.scss';
@use './../../assets/variables.scss';

.header {
  font-size: 1.2rem;
  position: sticky;
  top: 0;
  z-index: 10;
  border-style: none;
  background-color: inherit;
  // background-color: #121212;
}

.p-button {
  margin: 0 variables.$space-s;
}
</style>
