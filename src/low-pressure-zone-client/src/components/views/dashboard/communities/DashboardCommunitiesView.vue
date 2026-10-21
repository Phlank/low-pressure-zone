<template>
  <div class="communities-dashboard">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="communities">Communities</Tab>
        <Tab value="relationships" v-if="communities.communities.length > 0"> Relationships </Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="communities">
          <CommunitiesTab />
        </TabPanel>
        <TabPanel value="relationships" v-if="communities.communities.length > 0">
          <CommunityRelationshipsTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'openvue'
import { ref } from 'vue'
import CommunityRelationshipsTab from '@/components/views/dashboard/communities/CommunityRelationshipsTab.vue'
import CommunitiesTab from '@/components/views/dashboard/communities/CommunitiesTab.vue'
import {useCommunityStore} from "@/stores/communityStore.ts";

const communities = useCommunityStore()

const tabValue = ref('communities')
</script>
