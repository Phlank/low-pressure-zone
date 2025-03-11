<template>
  <div class="invites-grid">
    <DataTable
      :value="invites"
      key="id">
      <Column
        header="Email"
        field="email" />
      <Column header="Invited At">
        <template #body="{ data }: { data: InviteResponse }">
          {{ parseDate(data.invitedAt).toLocaleString() }}
        </template>
      </Column>
      <Column header="Last Sent At">
        <template #body="{ data }: { data: InviteResponse }">
          {{ parseDate(data.lastSentAt).toLocaleString() }}
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import type { InviteResponse } from '@/api/users/invites/inviteResponse'
import { parseDate } from '@/utils/dateUtils'
import { Column, DataTable } from 'primevue'

const props = defineProps<{ invites: InviteResponse[] }>()
</script>
