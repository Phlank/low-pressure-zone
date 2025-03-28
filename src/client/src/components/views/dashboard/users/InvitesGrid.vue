<template>
  <div class="invites-grid">
    <DataTable
      v-if="!isMobile"
      key="id"
      :value="invites">
      <Column
        field="email"
        header="Email" />
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
    <DataView
      v-if="isMobile"
      :paginator="communityStore.getRelationships(props.community.id).length > 5"
      :rows="5"
      :value="communityStore.getRelationships(props.community.id)"
      data-key="id"></DataView>
    <div
      v-for="(invite, index) in invites"
      v-else
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
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { Column, DataTable, DataView, Divider } from 'primevue'
import { inject, type Ref } from 'vue'
import type { InviteResponse } from '@/api/resources/invitesApi.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

defineProps<{ invites: InviteResponse[] }>()
</script>
