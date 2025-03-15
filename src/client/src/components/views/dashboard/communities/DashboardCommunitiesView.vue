<template>
  <div class="communities-dashboard">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab
          v-if="!useAuthStore().isInAnySpecifiedRole(Role.Admin)"
          value="mine">
          Mine
        </Tab>
        <Tab value="all"> All</Tab>
        <Tab
          v-if="authStore.isInAnySpecifiedRole(Role.Admin)"
          value="create">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel
          v-if="!useAuthStore().isInAnySpecifiedRole(Role.Admin)"
          value="mine">
          <div class="communities-dashboard__linked">
            <h4>Your Communities</h4>
            <CommunitiesGrid
              v-if="linkedCommunities.length > 0"
              :communities="communities.filter((a) => a.isRelated)"
              @deleted="loadCommunities()"
              @edited="loadCommunities()" />
            <div v-else>You do not currently have any linked communities.</div>
          </div>
        </TabPanel>
        <TabPanel value="all">
          <div class="communities-dashboard__all">
            <h4>All Communities</h4>
            <CommunitiesGrid
              :communities="communities"
              @deleted="loadCommunities()"
              @edited="loadCommunities()" />
          </div>
        </TabPanel>
        <TabPanel value="create">
          <CreateCommunity @created-community="loadCommunities()" />
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
import { Role } from '@/constants/roles'
import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi.ts'

const authStore = useAuthStore()
const isLoaded = ref(false)
const communities: Ref<CommunityResponse[]> = ref([])
const linkedCommunities = computed(() => communities.value.filter((a) => a.isLinkableToSchedule))
const tabValue: Ref<string | number> = ref('0')

onMounted(async () => {
  if (authStore.isInAnySpecifiedRole(Role.Admin)) tabValue.value = 'all'
  await loadCommunities()
})

const loadCommunities = async () => {
  console.log('loading communities')
  const response = await communitiesApi.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  communities.value = response.data!
}
</script>
