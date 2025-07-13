<template>
  <div class="active-tab">
    <UsersGrid :users="users" />
  </div>
</template>

<script lang="ts" setup>
import UsersGrid from '@/components/views/dashboard/users/UsersGrid.vue'
import { useToast } from 'primevue'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { showCreateSuccessToast } from '@/utils/toastUtils.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { onMounted, ref, type Ref } from 'vue'

const users: Ref<UserResponse[]> = ref([])
const toast = useToast()

onMounted(async () => {
  const userResponse = await usersApi.get()
  if (tryHandleUnsuccessfulResponse(userResponse, toast)) {
    return
  }
  users.value = userResponse.data()
})

const showConfirmDialog = ref(false)
const isSubmittingConfirmDialog = ref(false)
const handleCreateStreamers = async () => {
  isSubmittingConfirmDialog.value = true
  const response = await usersApi.createStreamers()
  if (response.isSuccess()) {
    showCreateSuccessToast(toast, 'Streamers', 'All')
  } else {
    tryHandleUnsuccessfulResponse(response, toast)
  }
  showConfirmDialog.value = false
  isSubmittingConfirmDialog.value = false
}
</script>
