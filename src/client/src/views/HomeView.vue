<template>
  <main>
    <h2 v-if="!isMobile">DJ schedule for 11 December 2024</h2>
    <h3 v-else>DJ schedule for 11 December 2024</h3>
    <table>
      <tr>
        <td>7</td>
        <td>Skolla</td>
      </tr>
      <tr>
        <td>8</td>
        <td>Phlank</td>
      </tr>
      <tr>
        <td>9</td>
        <td>Strgirll</td>
      </tr>
      <tr>
        <td>10</td>
        <td>Two Sevenz</td>
      </tr>
    </table>
  </main>
</template>

<script setup lang="ts">
import { inject } from 'vue'

const isMobile = inject('isMobile')
</script>

<style lang="scss" scoped>
@import '@/assets/variables.scss';
td {
  padding: $space-s $space-m;
}
</style>
