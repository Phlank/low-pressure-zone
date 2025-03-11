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
        <TabPanel value="pending">
          <InvitesGrid :invites="invites" />
        </TabPanel>
        <TabPanel value="invite">
          <InviteUser />
        </TabPanel>
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
import type { InviteResponse } from '@/api/users/invites/inviteResponse'
import type { UserResponse } from '@/api/users/userResponse'
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs, useToast } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import InvitesGrid from './InvitesGrid.vue'
import InviteUser from './InviteUser.vue'
import UsersGrid from './UsersGrid.vue'

const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('users')
const toast = useToast()

const users: Ref<UserResponse[]> = ref([])
const invites: Ref<InviteResponse[]> = ref([])

onMounted(async () => {
  const userResponse = await api.users.get()
  const inviteResponse = await api.users.invites.get()
  if (
    tryHandleUnsuccessfulResponse(userResponse, toast) ||
    tryHandleUnsuccessfulResponse(inviteResponse, toast)
  )
    return
  users.value = userResponse.data!
  invites.value = inviteResponse.data!
  isLoaded.value = true
})
</script>
