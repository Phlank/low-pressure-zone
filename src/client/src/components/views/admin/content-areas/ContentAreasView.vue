<template>
  <div class="content-areas-view">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="about">About</Tab>
        <Tab value="welcome">Welcome</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="about">
          <AboutContentTab />
        </TabPanel>
        <TabPanel value="welcome">
          <WelcomeContentTab />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref } from 'vue'
import AboutContentTab from '@/components/views/admin/content-areas/AboutContentTab.vue'
import WelcomeContentTab from '@/components/views/admin/content-areas/WelcomeContentTab.vue'

const tabValue = ref('about')
</script>
