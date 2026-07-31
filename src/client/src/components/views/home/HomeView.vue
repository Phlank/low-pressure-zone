<template>
  <div class="home-view">
    <Panel class="home-view__panel">
      <Tabs
        v-model:value="tabValue"
        scrollable>
        <TabList>
          <Tab value="schedule">Schedule</Tab>
          <Tab value="news">News</Tab>
          <Tab value="releases">Releases</Tab>
        </TabList>
        <TabPanels>
          <TabPanel value="schedule">
            <ScheduleTab />
          </TabPanel>
          <TabPanel value="news">
            <NewsTab />
          </TabPanel>
          <TabPanel value="releases"> </TabPanel>
        </TabPanels>
      </Tabs>
    </Panel>
  </div>
</template>

<script lang="ts" setup>
import { Panel, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import NewsTab from '@/components/views/home/NewsTab.vue'
import ScheduleTab from '@/components/views/home/ScheduleTab.vue'
import { ref } from 'vue'

const tabValue = ref('schedule')
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.home-view {
  margin: 0 variables.$space-m;
  display: flex;
  flex-direction: row;
  justify-content: center;

  &__panel {
    width: min(100%, calc(#{variables.$mobile-element-width} + 200px));

    .p-panel-header {
      display: none;
    }
  }
}
</style>
