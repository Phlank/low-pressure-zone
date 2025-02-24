<template>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--small">
      <Select
        class="input__field"
        id="audienceSelect"
        placeholder="Select an audience"
        option-label="name"
        option-value="id"
        :options="audiences"
        :disabled="disabled"
        :invalid="!validation.isValid('audienceId')"
        :model-value="formState.audienceId"
        @update:model-value="handleUpdateAudience" />
      <ValidationLabel
        for="audienceSelect"
        :message="validation.message('audienceId')">
        Audience
      </ValidationLabel>
    </IftaLabel>
  </div>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--medium">
      <DatePicker
        class="input__field"
        id="startTime"
        hourFormat="12"
        :min-date="minimumDate(formState.startTime, minStartTime)"
        :model-value="formState.startTime"
        :disabled="disabled || formState.startTime.getTime() < minStartTime.getTime()"
        :invalid="!validation.isValid('start')"
        @update:model-value="handleUpdateStart"
        show-time
        fluid />
      <ValidationLabel
        for="startTime"
        :message="validation.message('start')">
        Start Time
      </ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input input--medium">
      <DatePicker
        class="input__field"
        id="startTime"
        hourFormat="12"
        :min-date="formState.startTime"
        :max-date="new Date(formState.startTime.getTime() + MAX_DURATION_MINUTES * MS_PER_MINUTE)"
        :model-value="formState.endTime"
        :invalid="!validation.isValid('end')"
        :disabled="disabled || formState.endTime.getTime() < minStartTime.getTime()"
        @update:model-value="handleUpdateEnd"
        show-time
        fluid />
      <ValidationLabel
        for="endTime"
        :message="validation.message('end')">
        End Time
      </ValidationLabel>
    </IftaLabel>
  </div>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--xl">
      <Textarea
        class="input__field"
        id="descriptionInput"
        v-model:model-value="formState.description"
        auto-resize />
      <ValidationLabel
        for="descriptionInput"
        message=""
        text="Description"
        optional />
    </IftaLabel>
  </div>
</template>

<script lang="ts" setup>
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import { DatePicker, IftaLabel, Select, Textarea } from 'primevue'
import ValidationLabel from '../ValidationLabel.vue'
import { onMounted, reactive, ref, watch } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation'
import { MS_PER_MINUTE } from '@/constants/times'
import type { ScheduleRequest } from '@/api/schedules/scheduleRequest'
import { setToHour, minimumDate } from '@/utils/dateUtils'
import { scheduleRequestRules } from '@/validation/requestRules'

const MAX_DURATION_MINUTES = 1440
const DEFAULT_MINUTES = 60

let now = new Date()
const resetStartTime = new Date(
  now.getFullYear(),
  now.getMonth(),
  now.getDate(),
  now.getHours() + 1,
  0,
  0,
  0
)
const minStartTime = new Date(
  resetStartTime.getFullYear(),
  resetStartTime.getMonth(),
  resetStartTime.getDate() - 1,
  resetStartTime.getHours() + 1,
  0,
  0,
  0
)

export interface ScheduleFormState extends ScheduleRequest {
  startTime: Date
  endTime: Date
}

const formState: ScheduleFormState = reactive({
  audienceId: '',
  startTime: resetStartTime,
  start: resetStartTime.toISOString(),
  description: '',
  endTime: new Date(resetStartTime.getTime() + DEFAULT_MINUTES * MS_PER_MINUTE),
  end: new Date(resetStartTime.getTime() + DEFAULT_MINUTES * MS_PER_MINUTE).toISOString()
})
const startRef = ref(formState.start)
const validation = createFormValidation(formState, scheduleRequestRules(formState))

const props = defineProps<{
  initialState?: ScheduleFormState
  audiences: AudienceResponse[]
  disabled: boolean
}>()

onMounted(() => {
  if (props.initialState != undefined) {
    formState.audienceId = props.initialState.audienceId
    formState.start = props.initialState.start
    formState.end = props.initialState.end
    formState.description = props.initialState.description
    formState.startTime = props.initialState.startTime
    formState.endTime = props.initialState.endTime
  }
})

const handleUpdateAudience = (value: string) => {
  formState.audienceId = value
  validation.validateIfDirty('audienceId')
}

const handleUpdateStart = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  const duration = formState.endTime.getTime() - formState.startTime.getTime()
  const newStart = value as Date
  setToHour(newStart)
  formState.startTime = newStart
  formState.start = formState.startTime.toISOString()
  formState.endTime = new Date(formState.startTime.getTime() + duration)
  formState.end = formState.endTime.toISOString()
  validation.validateIfDirty('start')
  validation.validateIfDirty('end')
}

const handleUpdateEnd = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  const newEnd = value as Date
  setToHour(newEnd)
  formState.endTime = newEnd
  formState.end = formState.endTime.toISOString()
  validation.validateIfDirty('end')
}

const reset = () => {
  now = new Date()
  formState.audienceId = ''
  formState.startTime = resetStartTime
  formState.start = formState.startTime.toISOString()
  formState.description = ''
  formState.endTime = new Date(resetStartTime.getTime() + DEFAULT_MINUTES * MS_PER_MINUTE)
  formState.end = formState.endTime.toISOString()
}

defineExpose({ formState, validation, reset })

watch(
  () => formState.start,
  (newValue: string) => {
    startRef.value = newValue
  }
)
</script>
