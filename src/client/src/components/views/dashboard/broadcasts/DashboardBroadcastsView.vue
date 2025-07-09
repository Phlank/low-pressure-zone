<template>
  <div class="dashboard-broadcasts-view">
    <BroadcastsGrid />
  </div>
</template>

<script lang="ts" setup>
import { onMounted } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import BroadcastsGrid from '@/components/views/dashboard/broadcasts/BroadcastsGrid.vue'

const broadcastStore = useBroadcastStore()

onMounted(async () => {
  await broadcastStore.loadIfNotInitialized()
})
</script>
