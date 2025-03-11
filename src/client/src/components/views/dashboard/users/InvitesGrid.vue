<template>
  <div class="invites-grid">
    <DataTable
      v-if="!isMobile"
      :value="invites"
      key="id">
      <Column
        header="Email"
        field="email" />
      <Column header="Invited On">
        <template #body="{ data }: { data: InviteResponse }">
          {{ parseDate(data.invitedAt).toLocaleString() }}
        </template>
      </Column>
      <Column header="Last Sent On">
        <template #body="{ data }: { data: InviteResponse }">
          {{ parseDate(data.lastSentAt).toLocaleString() }}
        </template>
      </Column>
    </DataTable>
    <div
      v-else
      v-for="(invite, index) in invites"
      :key="invite.id">
      <ListItem>
        <template #left>
          <div style="display: flex; flex-direction: column">
            <span>{{ parseDate(invite.invitedAt).toLocaleDateString() }}</span>
            <span class="ellipsis text-s">{{ invite.email }}</span>
          </div>
        </template>
      </ListItem>
      <Divider v-if="index !== invites.length - 1" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import type { InviteResponse } from '@/api/users/invites/inviteResponse'
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { Column, DataTable, Divider } from 'primevue'
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = defineProps<{ invites: InviteResponse[] }>()
</script>
