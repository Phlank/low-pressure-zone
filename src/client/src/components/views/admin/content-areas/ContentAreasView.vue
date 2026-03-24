<template>
  <div class="content-areas-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="about">About</Tab>
        <Tab value="welcome">Welcome</Tab>
        <Tab value="privacy-policy">Privacy Policy</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="about">
          <AboutContentTab />
        </TabPanel>
        <TabPanel value="welcome">
          <WelcomeContentTab />
        </TabPanel>
        <TabPanel value="privacy-policy">
          <PrivacyPolicyTab />
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
import PrivacyPolicyTab from "@/components/views/admin/content-areas/PrivacyPolicyTab.vue";

const tabValue = ref('about')
</script>
