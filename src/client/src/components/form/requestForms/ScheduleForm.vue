<template>
  <div class="schedule-form">
    <FormArea>
      <IftaFormField
        :message="validation.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="m">
        <Select
          id="communityInput"
          v-model:model-value="formState.communityId"
          :disabled="isSubmitting || props.schedule?.community.id !== undefined"
          :invalid="!validation.isValid('communityId')"
          :options="availableCommunities"
          autofocus
          option-label="name"
          option-value="id"
          placeholder="Select an community"
          @update:model-value="validation.validateIfDirty('communityId')" />
      </IftaFormField>
    </FormArea>
    <FormArea>
      <IftaFormField
        :message="validation.message('startsAt')"
        input-id="startTimeInput"
        label="Start Time"
        size="m">
        <DatePicker
          id="startTimeInput"
          :disabled="formState.startTime.getTime() < minStartTime.getTime() || isSubmitting"
          :invalid="!validation.isValid('startsAt')"
          :min-date="minimumDate(formState.startTime, minStartTime)"
          :model-value="formState.startTime"
          fluid
          hourFormat="12"
          show-time
          @update:model-value="(value) => handleStartTimeChange(value as Date)" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('endsAt')"
        input-id="endTimeInput"
        label="End Time"
        size="m">
        <DatePicker
          id="endTimeInput"
          v-model:model-value="formState.endTime"
          :disabled="formState.endTime.getTime() < minStartTime.getTime() || isSubmitting"
          :invalid="!validation.isValid('endsAt')"
          :max-date="new Date(formState.startTime.getTime() + maxDurationMinutes * MS_PER_MINUTE)"
          :min-date="formState.startTime"
          fluid
          hourFormat="12"
          show-time
          @update:model-value="validation.validateIfDirty('endsAt')" />
      </IftaFormField>
    </FormArea>
    <FormArea :align-actions="alignActions">
      <IftaFormField
        :message="validation.message('description')"
        input-id="descriptionInput"
        label="Description"
        optional
        size="xl">
        <Textarea
          id="descriptionInput"
          v-model:model-value="formState.description"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('description')"
          auto-resize />
      </IftaFormField>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { DatePicker, Select, Textarea } from 'primevue'
import { computed, onMounted, type Ref, ref } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation'
import { MS_PER_MINUTE } from '@/constants/times'
import { minimumDate, parseDate } from '@/utils/dateUtils'
import { scheduleRequestRules } from '@/validation/requestRules'
import { type ScheduleRequest, type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import type { Result } from '@/types/result.ts'

const maxDurationMinutes = 1440
const defaultMinutes = 60
const schedules = useScheduleStore()

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

interface ScheduleFormState extends ScheduleRequest {
  startTime: Date
  endTime: Date
}

const formState: Ref<ScheduleFormState> = ref({
  communityId: '',
  startTime: computed({
    get: () => parseDate(formState.value.startsAt),
    set: (value) => {
      formState.value.startsAt = value.toISOString()
    }
  }),
  startsAt: resetStartTime.toISOString(),
  description: '',
  endTime: computed({
    get: () => parseDate(formState.value.endsAt),
    set: (value) => {
      formState.value.endsAt = value.toISOString()
    }
  }),
  endsAt: new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE).toISOString()
})
const validation = createFormValidation(formState, scheduleRequestRules(formState.value))

const handleStartTimeChange = (value: Date) => {
  formState.value.startTime = value
  if (formState.value.endTime < formState.value.startTime) {
    formState.value.endTime = new Date(value.getTime() + defaultMinutes * MS_PER_MINUTE)
  } else if (
    formState.value.endTime.getTime() >
    formState.value.startTime.getTime() + maxDurationMinutes * MS_PER_MINUTE
  ) {
    formState.value.endTime = new Date(value.getTime() + maxDurationMinutes * MS_PER_MINUTE)
  }
  validation.validateIfDirty('startsAt')
}

const props = withDefaults(
  defineProps<{
    schedule?: ScheduleResponse
    availableCommunities: CommunityResponse[]
    alignActions?: 'left' | 'right'
  }>(),
  {
    alignActions: 'left'
  }
)

const isSubmitting = ref(false)
const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  let response: Result
  if (props.schedule)
    response = await schedules.updateSchedule(props.schedule.id, formState, validation)
  else response = await schedules.createSchedule(formState, validation)
  isSubmitting.value = false
  if (!response.isSuccess) return
  emits('submitted')
  reset()
}

const reset = () => {
  now = new Date()
  formState.value.communityId = props.schedule?.community.id ?? ''
  formState.value.startsAt = props.schedule?.startsAt ?? resetStartTime.toISOString()
  formState.value.description = props.schedule?.description ?? ''
  formState.value.endsAt =
    props.schedule?.endsAt ??
    new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE).toISOString()
}

defineExpose({
  formState,
  validation,
  isSubmitting,
  reset,
  submit
})

const emits = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
