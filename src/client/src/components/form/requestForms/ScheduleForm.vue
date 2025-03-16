<template>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--small">
      <Select
        id="communitySelect"
        :invalid="!validation.isValid('communityId')"
        :model-value="formState.communityId"
        :options="communities"
        class="input__field"
        option-label="name"
        option-value="id"
        placeholder="Select an community"
        @update:model-value="handleUpdateCommunity" />
      <ValidationLabel
        :message="validation.message('communityId')"
        for="communitySelect">
        Community
      </ValidationLabel>
    </IftaLabel>
  </div>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--medium">
      <DatePicker
        id="startTime"
        :disabled="formState.startTime.getTime() < minStartTime.getTime()"
        :invalid="!validation.isValid('startsAt')"
        :min-date="minimumDate(formState.startTime, minStartTime)"
        :model-value="formState.startTime"
        class="input__field"
        fluid
        hourFormat="12"
        show-time
        @update:model-value="handleUpdateStart" />
      <ValidationLabel
        :message="validation.message('startsAt')"
        for="startTime">
        Start Time
      </ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input input--medium">
      <DatePicker
        id="startTime"
        :disabled="formState.endTime.getTime() < minStartTime.getTime()"
        :invalid="!validation.isValid('endsAt')"
        :max-date="new Date(formState.startTime.getTime() + maxDurationMinutes * MS_PER_MINUTE)"
        :min-date="formState.startTime"
        :model-value="formState.endTime"
        class="input__field"
        fluid
        hourFormat="12"
        show-time
        @update:model-value="handleUpdateEnd" />
      <ValidationLabel
        :message="validation.message('endsAt')"
        for="endTime">
        End Time
      </ValidationLabel>
    </IftaLabel>
  </div>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--xl">
      <Textarea
        id="descriptionInput"
        v-model:model-value="formState.description"
        auto-resize
        class="input__field" />
      <ValidationLabel
        for="descriptionInput"
        message=""
        optional
        text="Description" />
    </IftaLabel>
  </div>
</template>

<script lang="ts" setup>
import { DatePicker, IftaLabel, Select, Textarea } from 'primevue'
import ValidationLabel from '../ValidationLabel.vue'
import { onMounted, reactive, ref, watch } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation'
import { MS_PER_MINUTE } from '@/constants/times'
import { minimumDate, setToHour } from '@/utils/dateUtils'
import { scheduleRequestRules } from '@/validation/requestRules'
import type { ScheduleRequest } from '@/api/resources/schedulesApi.ts'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'

const maxDurationMinutes = 1440
const defaultMinutes = 60

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
  communityId: '',
  startTime: resetStartTime,
  startsAt: resetStartTime.toISOString(),
  description: '',
  endTime: new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE),
  endsAt: new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE).toISOString()
})
const startRef = ref(formState.startsAt)
const validation = createFormValidation(formState, scheduleRequestRules(formState))

const props = defineProps<{
  initialState?: ScheduleFormState
  communities: CommunityResponse[]
}>()

onMounted(() => {
  if (props.initialState !== undefined) {
    formState.communityId = props.initialState.communityId
    formState.startsAt = props.initialState.startsAt
    formState.endsAt = props.initialState.endsAt
    formState.description = props.initialState.description
    formState.startTime = props.initialState.startTime
    formState.endTime = props.initialState.endTime
  }
})

const handleUpdateCommunity = (value: string) => {
  formState.communityId = value
  validation.validateIfDirty('communityId')
}

const handleUpdateStart = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  const duration = formState.endTime.getTime() - formState.startTime.getTime()
  const newStart = value as Date
  setToHour(newStart)
  formState.startTime = newStart
  formState.startsAt = formState.startTime.toISOString()
  formState.endTime = new Date(formState.startTime.getTime() + duration)
  formState.endsAt = formState.endTime.toISOString()
  validation.validateIfDirty('startsAt')
  validation.validateIfDirty('endsAt')
}

const handleUpdateEnd = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  const newEnd = value as Date
  setToHour(newEnd)
  formState.endTime = newEnd
  formState.endsAt = formState.endTime.toISOString()
  validation.validateIfDirty('endsAt')
}

const reset = () => {
  now = new Date()
  formState.communityId = ''
  formState.startTime = resetStartTime
  formState.startsAt = formState.startTime.toISOString()
  formState.description = ''
  formState.endTime = new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE)
  formState.endsAt = formState.endTime.toISOString()
}

defineExpose({ formState, validation, reset })

watch(
  () => formState.startsAt,
  (newValue: string) => {
    startRef.value = newValue
  }
)
</script>
