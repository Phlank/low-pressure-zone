<template>
  <div class="audiences-dashboard">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="0"> Mine </Tab>
        <Tab value="1"> All </Tab>
        <Tab
          value="2"
          v-if="authStore.isInAnySpecifiedRole(Role.Admin)">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="0">
          <div class="audiences-dashboard__linked">
            <h4>Your Audiences</h4>
            <AudiencesGrid
              v-if="linkedAudiences.length > 0"
              :audiences="audiences.filter((a) => a.isLinkableToSchedule)"
              @edit-success="loadAudiences()"
              @delete-success="loadAudiences()" />
            <div v-else>You do not currently have any linked audiences.</div>
          </div>
        </TabPanel>
        <TabPanel value="1">
          <div class="audiences-dashboard__all">
            <h4>All Audiences</h4>
            <AudiencesGrid
              :audiences="audiences"
              @edit-success="loadAudiences()"
              @delete-success="loadAudiences()" />
          </div>
        </TabPanel>
        <TabPanel value="2">
          <CreateAudience @created-audience="loadAudiences()" />
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
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { computed, onMounted, ref, type Ref } from 'vue'
import AudiencesGrid from './AudiencesGrid.vue'
import CreateAudience from './CreateAudience.vue'
import { useAuthStore } from '@/stores/authStore'
import { Role } from '@/constants/roles'

const authStore = useAuthStore()
const isLoaded = ref(false)
const audiences: Ref<AudienceResponse[]> = ref([])
const linkedAudiences = computed(() => audiences.value.filter((a) => a.isLinkableToSchedule))
const tabValue: Ref<string | number> = ref('0')

onMounted(async () => {
  await loadAudiences()
})

const loadAudiences = async () => {
  const response = await api.audiences.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  audiences.value = response.data!
}
</script>
