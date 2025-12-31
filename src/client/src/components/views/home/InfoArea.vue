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
          <AboutSection />
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
import AboutSection from '@/components/views/home/AboutSection.vue'
import NewsTab from '@/components/views/home/NewsTab.vue'

const tabValue = ref('about')
</script>
