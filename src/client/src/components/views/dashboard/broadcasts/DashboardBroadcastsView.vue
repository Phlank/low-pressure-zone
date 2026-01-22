<template>
  <div class="dashboard-broadcasts-view">
    <p>Broadcasts are automatically deleted after two weeks to prevent running out of storage on the radio server.</p>
    <BroadcastsGrid :show-streamer-name="authStore.isInAnyRoles(roles.admin, roles.organizer)" />
  </div>
</template>

<script lang="ts" setup>
import BroadcastsGrid from '@/components/views/dashboard/broadcasts/BroadcastsGrid.vue'
import { useAuthStore } from '@/stores/authStore.ts'
import { roles } from '@/constants/roles.ts'

const authStore = useAuthStore()
</script>
