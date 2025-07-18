<template>
  <div class="dashboard-users">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="users">Active</Tab>
        <Tab value="pending">Pending</Tab>
        <Tab value="invite">Invite</Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="users">
          <ActiveTab />
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
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs, useToast } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import InvitesGrid from './InvitesGrid.vue'
import InviteUser from './InviteUser.vue'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import invitesApi, { type InviteResponse } from '@/api/resources/invitesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import ActiveTab from '@/components/views/dashboard/users/ActiveTab.vue'

const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('users')
const toast = useToast()

const users: Ref<UserResponse[]> = ref([])
const invites: Ref<InviteResponse[]> = ref([])

onMounted(async () => {
  const userResponse = await usersApi.get()
  const inviteResponse = await invitesApi.get()
  if (
    tryHandleUnsuccessfulResponse(userResponse, toast) ||
    tryHandleUnsuccessfulResponse(inviteResponse, toast)
  )
    return
  users.value = userResponse.data()
  invites.value = inviteResponse.data()
  isLoaded.value = true
})
</script>
