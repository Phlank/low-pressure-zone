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
      <Column class="grid-actions-col grid-action-col--1">
        <template #body="{ data }: { data: InviteResponse }">
          <GridActions
            show-resend
            @resend="handleResendActionClick(data)" />
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
                @resend="handleResendActionClick(invite)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
    </DataView>
    <Dialog
      :visible="showResendDialog"
      closable
      header="Resend Invite"
      modal
      @hide="handleHideShowResendDialog">
      <template #default> Resend invitation to {{ resendEmail }}?</template>
      <template #footer>
        <Button
          :disabled="isSending"
          :loading="isSending"
          label="Resend"
          @click="handleResendInvite" />
      </template>
    </Dialog>
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { Button, Column, DataTable, DataView, Dialog, Divider, useToast } from 'primevue'
import { inject, onMounted, ref, type Ref } from 'vue'
import invitesApi, { type InviteResponse } from '@/api/resources/invitesApi.ts'
import { useInviteStore } from '@/stores/useInviteStore.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { showApiStatusToast } from '@/utils/toastUtils.ts'
import { parseDate } from '@/utils/dateUtils.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const inviteStore = useInviteStore()
const communityStore = useCommunityStore()
const toast = useToast()

onMounted(async () => {
  const promises: Promise<void>[] = []
  if (inviteStore.invites.length === 0) promises.push(inviteStore.loadInvitesAsync())
  if (communityStore.communities.length === 0) promises.push(communityStore.loadCommunitiesAsync())
  await Promise.all(promises)
})

const showResendDialog = ref(false)
const resendEmail = ref('')
const resendId = ref('')
const isSending = ref(false)

const handleResendActionClick = async (invite: InviteResponse) => {
  showResendDialog.value = true
  resendEmail.value = invite.email
  resendId.value = invite.id
}

const handleResendInvite = async () => {
  isSending.value = true
  const response = await invitesApi.postResend(resendId.value)
  if (!response.isSuccess()) {
    showApiStatusToast(toast, response.status)
  }
  showResendDialog.value = false
  isSending.value = false
}

const handleHideShowResendDialog = () => {
  console.log('hide')
  showResendDialog.value = false
}
</script>
