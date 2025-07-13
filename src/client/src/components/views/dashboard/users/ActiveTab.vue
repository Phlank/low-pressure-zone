<template>
  <div class="active-tab">
    <UsersGrid :users="users" />
  </div>
</template>

<script lang="ts" setup>
import UsersGrid from '@/components/views/dashboard/users/UsersGrid.vue'
import { useToast } from 'primevue'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
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
</script>
