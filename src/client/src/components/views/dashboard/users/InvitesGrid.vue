<template>
  <div class="invites-grid">
    <DataTable
      v-if="!isMobile"
      key="id"
      :value="inviteStore.invites">
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
      v-else
      :paginator="inviteStore.invites.length > 5"
      :rows="5"
      :value="inviteStore.invites"
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
                @resend="handleResendInvite(invite)" />
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
import { parseDate } from '@/utils/dateUtils'
import { Column, DataTable, DataView, Divider } from 'primevue'
import { inject, onMounted, type Ref } from 'vue'
import type { InviteResponse } from '@/api/resources/invitesApi.ts'
import { useInviteStore } from '@/stores/useInviteStore.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const inviteStore = useInviteStore()
const communityStore = useCommunityStore()

onMounted(async () => {
  const promises: Promise<void>[] = []
  if (inviteStore.invites.length === 0) promises.push(inviteStore.loadInvitesAsync())
  if (communityStore.communities.length === 0) promises.push(communityStore.loadCommunitiesAsync())
  await Promise.all(promises)
})

const handleResendInvite = async (invite: InviteResponse) => {}
</script>
