<template>
  <div class="content-areas-view">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="about">About</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="about">
          <AboutContentTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref } from 'vue'
import AboutContentTab from '@/components/views/admin/content-areas/AboutContentTab.vue'

const tabValue = ref('about')
</script>
