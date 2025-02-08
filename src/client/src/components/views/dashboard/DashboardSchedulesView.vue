<template>
  <div>Schedules content</div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { onMounted } from 'vue'

onMounted(async () => {
  await api.schedules.get()
})
</script>
