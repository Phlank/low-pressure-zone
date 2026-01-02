<template>
  <div class="dashboard-broadcasts-view">
    <BroadcastsGrid
      :show-streamer-name="authStore.isInAnyRoles(Roles.admin, Roles.organizer)" />
  </div>
</template>

<script lang="ts" setup>
import { onMounted } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import BroadcastsGrid from '@/components/views/dashboard/broadcasts/BroadcastsGrid.vue'
import { useAuthStore } from '@/stores/authStore.ts'
import Roles from '@/constants/roles.ts'

const authStore = useAuthStore()
const broadcastStore = useBroadcastStore()

onMounted(async () => {
  await broadcastStore.loadIfNotInitialized()
})
</script>
