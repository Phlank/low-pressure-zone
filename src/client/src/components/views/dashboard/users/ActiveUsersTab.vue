<template>
  <div class="active-users-tab">
    <UsersGrid
      @create-streamer="handleCreateStreamer"
      @new-invite="handleNewInvite" />
    <FormDrawer
      v-model:visible="isDrawerVisible"
      :is-submitting="inviteFormRef?.isSubmitting"
      title="New Invitation"
      @reset="inviteFormRef?.reset"
      @submit="inviteFormRef?.submit">
      <InviteForm
        ref="inviteFormRef"
        hide-submit
        @submitted="isDrawerVisible = false" />
    </FormDrawer>
  </div>
</template>

<script lang="ts" setup>
import UsersGrid from '@/components/views/dashboard/users/UsersGrid.vue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { useUserStore } from '@/stores/userStore.ts'
import FormDrawer from '@/components/form/FormDrawer.vue'
import InviteForm from '@/components/form/requestForms/InviteForm.vue'
import { ref, useTemplateRef } from 'vue'

const users = useUserStore()

const handleCreateStreamer = async (user: UserResponse) => {
  await users.makeStreamer(user.id)
}

const isDrawerVisible = ref(false)
const inviteFormRef = useTemplateRef('inviteFormRef')
const handleNewInvite = () => {
  isDrawerVisible.value = true
}
</script>
