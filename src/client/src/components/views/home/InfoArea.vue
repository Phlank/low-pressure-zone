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

        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Tabs, TabList, Tab } from 'primevue'
import AboutSection from "@/components/views/home/AboutSection.vue";

const tabValue = ref('about')
</script>

<style scoped lang="scss"></style>
