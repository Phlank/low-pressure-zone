<template>
  <div class="communities-dashboard">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab
          v-if="!useAuthStore().isInRole(roles.admin)"
          value="mine">
          Mine
        </Tab>
        <Tab value="all">All</Tab>
        <Tab
          v-if="communityStore.organizableCommunities.length > 0"
          value="relationships">
          Relationships
        </Tab>
        <Tab
          v-if="authStore.isInRole(roles.admin)"
          value="create">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="mine">
          <div class="communities-dashboard__linked">
            <h4>Your Communities</h4>
            <CommunitiesGrid
              v-if="communityStore.relatedCommunities.length > 0"
              :communities="communityStore.relatedCommunities" />
            <div v-else>You do not currently have any linked communities.</div>
          </div>
        </TabPanel>
        <TabPanel value="all">
          <div class="communities-dashboard__all">
            <h4>All Communities</h4>
            <CommunitiesGrid :communities="communityStore.communities" />
          </div>
        </TabPanel>
        <TabPanel value="relationships">
          <div
            v-if="communityStore.organizableCommunities.length > 0"
            class="communities-dashboard__relationships">
            <CommunityRelationships
              :available-communities="communityStore.organizableCommunities" />
          </div>
        </TabPanel>
        <TabPanel value="create">
          <CreateCommunity />
        </TabPanel>
      </TabPanels>
    </Tabs>
    <Skeleton
      v-show="!isLoaded"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import CommunitiesGrid from './CommunitiesGrid.vue'
import CreateCommunity from './CreateCommunity.vue'
import { useAuthStore } from '@/stores/authStore'
import CommunityRelationships from '@/components/views/dashboard/communities/CommunityRelationships.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useUserStore } from '@/stores/userStore.ts'
import roles from '@/constants/roles.ts'

const authStore = useAuthStore()
const communityStore = useCommunityStore()
const userStore = useUserStore()
const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('mine')

onMounted(async () => {
  if (authStore.isInRole(roles.admin)) tabValue.value = 'all'
  const promises: Promise<void>[] = []
  if (communityStore.communities.length === 0) {
    promises.push(communityStore.loadCommunitiesAsync())
  }
  if (userStore.users.length === 0) {
    promises.push(userStore.loadUsersAsync())
  }
  await Promise.all(promises)
  isLoaded.value = true
})
</script>
