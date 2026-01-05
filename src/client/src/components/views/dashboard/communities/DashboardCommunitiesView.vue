<template>
  <div class="communities-dashboard">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="communities">Communities</Tab>
        <Tab value="relationships"> Relationships </Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="communities">
          <CommunitiesTab />
        </TabPanel>
        <TabPanel value="relationships">
          <CommunityRelationshipsTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref } from 'vue'
import CommunityRelationshipsTab from '@/components/views/dashboard/communities/CommunityRelationshipsTab.vue'
import CommunitiesTab from '@/components/views/dashboard/communities/CommunitiesTab.vue'

const tabValue = ref('communities')
</script>
