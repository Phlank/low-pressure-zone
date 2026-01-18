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
        style="gap: 0"
        id="scheduleDateInput"
        class="schedule-navigator__date-field"
        v-model:model-value="currentSchedule"
        :disabled="scheduleStore.isLoading"
        :loading="scheduleStore.isLoading"
        :options="selectOptions"
        empty-message="No upcoming schedules"
        overlay-class="schedule-navigator__date-field__overlay">
        <template #value="{ value }: { value: ScheduleResponse | undefined }">
          <ListItem
            v-if="value !== undefined"
            :hide-overflow-left="false"
            hide-overflow-right
            style="min-height: fit-content">
            <template #left>
              <TwoLineData
                :above="parseDate(value.startsAt).toLocaleDateString()"
                :below="value.community.name" />
            </template>
            <template #right>
              <span>{{ value.name }}</span>
            </template>
          </ListItem>
        </template>
        <template #option="{ option }: { option: ScheduleResponse | undefined }">
          <ListItem
            v-if="option !== undefined"
            :hide-overflow-left="false"
            hide-overflow-right
            style="min-height: fit-content">
            <template #left>
              <TwoLineData
                :above="parseDate(option.startsAt).toLocaleDateString()"
                :below="option.community.name" />
            </template>
            <template #right>
              <span>{{ option.name }}</span>
            </template>
          </ListItem>
        </template>
      </Select>
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
import { computed, type Ref, ref, watch } from 'vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormField from '@/components/form/FormField.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils.ts'
import TwoLineData from '@/components/layout/TwoLineData.vue'

const scheduleStore = useScheduleStore()

const selectOptions = computed(() => scheduleStore.upcomingSchedules)
const currentSchedule: Ref<ScheduleResponse | undefined> = ref(undefined)

const hasPrevious = computed(
  () => selectOptions.value.findIndex((option) => option == currentSchedule.value) > 0
)
const handlePreviousClick = () => {
  if (hasPrevious.value) {
    const index = selectOptions.value.findIndex((option) => option == currentSchedule.value)
    currentSchedule.value = selectOptions.value[index - 1]!
  }
}
const hasNext = computed(
  () =>
    selectOptions.value.findIndex((option) => option == currentSchedule.value) <
    selectOptions.value.length - 1
)
const handleNextClick = () => {
  if (hasNext.value) {
    const index = selectOptions.value.findIndex((option) => option == currentSchedule.value)
    currentSchedule.value = selectOptions.value[index + 1]!
  }
}

const emit = defineEmits<{
  changeSchedule: [string]
}>()

watch(currentSchedule, (newValue) => {
  emit('changeSchedule', newValue?.id ?? '')
})

watch(
  () => scheduleStore.upcomingSchedules,
  (newSchedules) => {
    if (newSchedules.length > 0 && !currentSchedule.value) {
      currentSchedule.value = newSchedules[0]!
      emit('changeSchedule', currentSchedule.value.id)
    }
  },
  { immediate: true }
)
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
    width: 390px;
    gap: 0 !important;

    @media (max-width: 450px) {
      width: calc(100dvw - 2 * #{variables.$space-m} - 2 * #{variables.$space-l});

      .p-select-label {
        display: inline;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
      }
    }

    &__two-line-data {
      gap: 0 !important;
    }

    &__overlay {
      min-width: unset !important;
      max-width: min(calc(100dvw - 2 * #{variables.$space-m} - 2 * #{variables.$space-l}), 390px);
      position: absolute !important;
    }
  }

  &__button {
    width: 32px;
    @media (max-width: 450px) {
      display: none;
    }
  }

  div.two-line-data {
    gap: 0;
  }
}
</style>
