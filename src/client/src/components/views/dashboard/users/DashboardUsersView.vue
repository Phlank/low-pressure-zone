<template>
  <div class="dashboard-users">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="active">Active</Tab>
        <Tab value="pending">Pending</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="active">
          <ActiveUsersTab />
        </TabPanel>
        <TabPanel value="pending">
          <PendingInvitesTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref, type Ref } from 'vue'
import ActiveUsersTab from '@/components/views/dashboard/users/ActiveUsersTab.vue'
import PendingInvitesTab from '@/components/views/dashboard/users/PendingInvitesTab.vue'

const tabValue: Ref<string> = ref('active')
</script>
