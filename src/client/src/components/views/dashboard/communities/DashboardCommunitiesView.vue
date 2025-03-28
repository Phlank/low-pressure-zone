<template>
  <div class="communities-dashboard">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab
          v-if="!useAuthStore().isInRole(Role.Admin)"
          value="mine">
          Mine
        </Tab>
        <Tab value="all">All</Tab>
        <Tab
          v-if="organizingCommunities.length > 0"
          value="relationships">
          Relationships
        </Tab>
        <Tab
          v-if="authStore.isInRole(Role.Admin)"
          value="create">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="mine">
          <div class="communities-dashboard__linked">
            <h4>Your Communities</h4>
            <CommunitiesGrid
              v-if="linkedCommunities.length > 0"
              :communities="
                communityStore.communities.filter(
                  (community) => community.isPerformable || community.isOrganizable
                )
              " />
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
            v-if="organizingCommunities.length > 0"
            class="communities-dashboard__relationships">
            <CommunityRelationships :available-communities="organizingCommunities" />
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
import { computed, onMounted, ref, type Ref } from 'vue'
import CommunitiesGrid from './CommunitiesGrid.vue'
import CreateCommunity from './CreateCommunity.vue'
import { useAuthStore } from '@/stores/authStore'
import { Role } from '@/constants/role.ts'
import CommunityRelationships from '@/components/views/dashboard/communities/CommunityRelationships.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useUserStore } from '@/stores/userStore.ts'

const authStore = useAuthStore()
const communityStore = useCommunityStore()
const userStore = useUserStore()
const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('mine')

const linkedCommunities = computed(() =>
  communityStore.communities.filter(
    (community) => community.isPerformable || community.isOrganizable
  )
)

const organizingCommunities = computed(() =>
  communityStore.communities.filter((community) => community.isOrganizable)
)

onMounted(async () => {
  if (authStore.isInRole(Role.Admin)) tabValue.value = 'all'
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
