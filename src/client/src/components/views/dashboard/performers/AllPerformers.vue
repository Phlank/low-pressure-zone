<template>
  <div class="all-performers">
    <h4>All Performers</h4>
    <PerformersGrid :performers="performers" />
  </div>
</template>

<script lang="ts" setup>
import type { PerformerResponse } from '@/api/performers/performerResponse'
import { Role } from '@/constants/roles'
import { useUserStore } from '@/stores/userStore'
import PerformersGrid from './PerformersGrid.vue'

const userStore = useUserStore()

const props = defineProps<{
  performers: PerformerResponse[]
}>()
</script>
