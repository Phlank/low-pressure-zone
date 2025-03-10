<template>
  <div class="invites-grid">
    <DataTable> </DataTable>
  </div>
</template>

<script lang="ts" setup>
import type { InviteResponse } from '@/api/users/invites/inviteResponse'
import { DataTable } from 'primevue'

const props = defineProps<{ invites: InviteResponse[] }>()
</script>
