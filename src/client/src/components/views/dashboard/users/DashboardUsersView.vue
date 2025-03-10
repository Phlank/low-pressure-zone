<template>
  <div class="dashboard-users">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="users">Users</Tab>
        <Tab value="pending">Pending</Tab>
        <Tab value="invite">Invite</Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="users">
          <UsersGrid :users="users" />
        </TabPanel>
        <TabPanel value="pending"></TabPanel>
        <TabPanel value="invite"></TabPanel>
      </TabPanels>
    </Tabs>
    <Skeleton
      v-show="!isLoaded"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { UserResponse } from '@/api/users/userResponse'
import { Tab, TabList, TabPanel, TabPanels, Tabs, useToast, Skeleton } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import UsersGrid from './UsersGrid.vue'

const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('users')
const toast = useToast()

const users: Ref<UserResponse[]> = ref([])

onMounted(async () => {
  const userResponse = await api.users.get()
  if (tryHandleUnsuccessfulResponse(userResponse, toast)) return
  users.value = userResponse.data!
  isLoaded.value = true
})
</script>
