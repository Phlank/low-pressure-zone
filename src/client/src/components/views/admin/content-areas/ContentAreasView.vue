<template>
  <div class="content-areas-view">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="about">About</Tab>
        <Tab value="news">News</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="about">
          <AboutTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref } from 'vue'
import AboutTab from '@/components/views/admin/content-areas/AboutTab.vue'

const tabValue = ref('about')
</script>
