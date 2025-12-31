<template>
  <div class="invites-grid">
    <DataTable
      v-if="!isMobile"
      key="id"
      :value="invites.items">
      <template #empty>No items to display.</template>
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
      <Column class="grid-actions-col grid-action-col--1">
        <template #body="{ data }: { data: InviteResponse }">
          <GridActions
            show-resend
            @resend="emit('resend', data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-else
      :paginator="invites.items.length > 5"
      :rows="5"
      :value="invites.items"
      data-key="id">
      <template #empty>
        <ListItem>
          <template #left>No items to display.</template>
        </ListItem>
      </template>
      <template #list="{ items }: { items: InviteResponse[] }">
        <div
          v-for="(invite, index) in items"
          :key="invite.id">
          <ListItem>
            <template #left>
              <span>{{ parseDate(invite.invitedAt).toLocaleDateString() }}</span>
              <span class="ellipsis text-s">{{ invite.email }}</span>
            </template>
            <template #right>
              <GridActions
                show-resend
                @resend="emit('resend', invite)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { Column, DataTable, DataView, Divider } from 'primevue'
import { inject, onMounted, type Ref } from 'vue'
import { type InviteResponse } from '@/api/resources/invitesApi.ts'
import { useInviteStore } from '@/stores/inviteStore.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { parseDate } from '@/utils/dateUtils.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const invites = useInviteStore()
const communityStore = useCommunityStore()

onMounted(async () => {
  if (communityStore.communities.length === 0) {
    await communityStore.loadCommunitiesAsync()
  }
})

const emit = defineEmits<{
  resend: [InviteResponse]
  create: []
}>()
</script>
