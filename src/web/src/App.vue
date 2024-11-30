<template>
  <SiteHeader />
  <RouterView />
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import SiteHeader from './components/shared/SiteHeader.vue'
</script>
