<template>
  <div class="info-area">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="about">About</Tab>
        <Tab value="news">News</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="about">
          <AboutTab />
        </TabPanel>
        <TabPanel value="news">
          <NewsTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import NewsTab from '@/components/views/home/NewsTab.vue'
import AboutTab from '@/components/views/home/AboutTab.vue'

const tabValue = ref('about')
</script>
