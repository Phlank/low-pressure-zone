<template>
  <div class="upcoming-schedules">
    <Skeleton
      v-if="schedules.isLoading"
      height="250px" />
    <div v-else>
      <ScheduleNavigator
        v-if="schedules.upcomingSchedules.length > 0"
        :schedules="schedules"
        @changeSchedule="handleChangeSchedule" />
      <div
        v-if="schedules.upcomingSchedules.length === 0"
        class="upcoming-schedules__content upcoming-schedules__content--none">
        No upcoming schedule to display.
      </div>
      <div
        v-else
        class="upcoming-schedules__content">
        <h3 v-if="schedule !== undefined && schedule.name !== ''">{{ schedule?.name }}</h3>
        <div
          v-if="schedule !== undefined && schedule.description !== ''"
          class="upcoming-schedules__content__description">
          <MarkdownContent :content="schedule.description" />
        </div>
        <HomeScheduleGrid :schedule-id="currentId" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Skeleton } from 'openvue'
import { computed, ref } from 'vue'
import ScheduleNavigator from '@/components/controls/ScheduleNavigator.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import MarkdownContent from "@/components/controls/MarkdownContent.vue";
import HomeScheduleGrid from '@/components/views/home/HomeScheduleGrid.vue'

const schedules = useScheduleStore()

const currentId = ref('')
const handleChangeSchedule = (newId: string) => {
  currentId.value = newId
}
const schedule = computed(() => schedules.getScheduleById(currentId.value))
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.upcoming-schedules {
  width: 100%;

  &__content {
    margin-top: variables.$space-m;
    text-align: center;

    &__description {
      padding: 0 0 variables.$space-l 0;
      text-align: left;
    }
  }
}
</style>
