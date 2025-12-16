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
          <NewsSection />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Tabs, TabList, Tab, TabPanels, TabPanel } from 'primevue'
import AboutSection from "@/components/views/home/AboutSection.vue";
import NewsSection from "@/components/views/home/NewsSection.vue";

const tabValue = ref('about')
</script>
