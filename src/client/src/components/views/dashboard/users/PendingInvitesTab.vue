<template>
  <div class="invites-tab">
    <InvitesGrid @resend="handleResendAction" />
    <ConfirmDialog
      v-model:visible="isResendDialogVisible"
      header="Resend Invite"
      :isSubmitting="isSending"
      text="Are you sure you want to resend this invite email?"
      @close="isResendDialogVisible = false"
      @confirm="handleResendConfirm" />
  </div>
</template>

<script lang="ts" setup>
import type { InviteResponse } from '@/api/resources/invitesApi.ts'
import { useInviteStore } from '@/stores/inviteStore.ts'
import { ref } from 'vue'
import InvitesGrid from '@/components/views/dashboard/users/InvitesGrid.vue'
import ConfirmDialog from '@/components/dialogs/ConfirmDialog.vue'

const invites = useInviteStore()

const isResendDialogVisible = ref(false)
const selectedInvite = ref<InviteResponse | null>(null)
const handleResendAction = async (invite: InviteResponse) => {
  isResendDialogVisible.value = true
  selectedInvite.value = invite
}
const isSending = ref(false)
const handleResendConfirm = async () => {
  if (!selectedInvite.value) return
  isSending.value = true
  const result = await invites.resendEmail(selectedInvite.value.id)
  isSending.value = false
  if (!result) return
  isResendDialogVisible.value = false
  selectedInvite.value = null
}
</script>
