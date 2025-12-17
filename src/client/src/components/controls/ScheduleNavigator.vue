<template>
  <div class="schedule-navigator">
    <FormField
      class="schedule-navigator__button"
      size="xs">
      <Button
        v-if="hasPrevious"
        :disabled="!hasPrevious"
        icon="pi pi-chevron-left"
        outlined
        rounded
        severity="secondary"
        size="small"
        @click="handlePreviousClick" />
    </FormField>
    <IftaFormField
      class="schedule-navigator__date-field"
      input-id="scheduleDateInput"
      label="Schedule">
      <Select
        id="scheduleDateInput"
        v-model:model-value="currentScheduleId"
        :disabled="isLoading"
        :loading="isLoading"
        :options="selectOptions"
        empty-message="No upcoming schedules"
        option-label="label"
        option-value="value" />
    </IftaFormField>
    <FormField
      class="schedule-navigator__button"
      size="xs">
      <Button
        v-show="hasNext"
        :disabled="!hasNext"
        :severity="'secondary'"
        icon="pi pi-chevron-right"
        outlined
        rounded
        size="small"
        @click="handleNextClick" />
    </FormField>
  </div>
</template>

<script lang="ts" setup>
import { Button, Select } from 'primevue'
import { computed, onMounted, type Ref, ref, watch } from 'vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormField from '@/components/form/FormField.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const scheduleStore = useScheduleStore()

const selectOptions = computed(() =>
  scheduleStore.upcomingSchedules.map((schedule) => ({
    label: `${new Date(schedule.startsAt).toLocaleDateString()} - ${schedule.community.name}`,
    value: schedule.id
  }))
)

const isLoading = ref(true)
const currentScheduleId: Ref<string | undefined> = ref(undefined)
const hasPrevious = computed(
  () => selectOptions.value.findIndex((option) => option.value == currentScheduleId.value) > 0
)
const handlePreviousClick = () => {
  if (hasPrevious.value) {
    const index = selectOptions.value.findIndex((option) => option.value == currentScheduleId.value)
    currentScheduleId.value = selectOptions.value[index - 1]!.value
  }
}
const hasNext = computed(
  () =>
    selectOptions.value.findIndex((option) => option.value == currentScheduleId.value) <
    selectOptions.value.length - 1
)
const handleNextClick = () => {
  if (hasNext.value) {
    const index = selectOptions.value.findIndex((option) => option.value == currentScheduleId.value)
    currentScheduleId.value = selectOptions.value[index + 1]!.value
  }
}

onMounted(async () => {
  await scheduleStore.loadDefaultSchedulesAsync()
  isLoading.value = false
  if (scheduleStore.upcomingSchedules.length > 0) {
    currentScheduleId.value = scheduleStore.upcomingSchedules[0]!.id
  }
})

const emit = defineEmits<{
  changeSchedule: [string]
}>()

watch(currentScheduleId, (newValue) => {
  emit('changeSchedule', newValue ?? '')
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.schedule-navigator {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 10px;
  justify-content: center;

  &__date-field {
    width: 275px;

    @media (max-width: 416px) {
      width: calc(100dvw - 2 * #{variables.$space-m} - 2 * #{variables.$space-l});

      .p-select-label {
        display: inline;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
      }
    }
  }

  &__button {
    width: 32px;
    @media (max-width: 416px) {
      display: none;
    }
  }
}
</style>
