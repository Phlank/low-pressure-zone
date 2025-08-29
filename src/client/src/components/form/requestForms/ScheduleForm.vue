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
          :disabled="isSubmitting"
          :invalid="!validation.isValid('communityId')"
          :options="communities"
          class="input__field"
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
          class="input__field"
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
          class="input__field"
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
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Button, DatePicker, Select, Textarea, useToast } from 'primevue'
import { computed, onMounted, type Ref, ref } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation'
import { MS_PER_MINUTE } from '@/constants/times'
import { minimumDate, parseDate } from '@/utils/dateUtils'
import { scheduleRequestRules } from '@/validation/requestRules'
import schedulesApi, { type ScheduleRequest } from '@/api/resources/schedulesApi.ts'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import type { ApiResponse } from '@/api/apiResponse.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const maxDurationMinutes = 1440
const defaultMinutes = 60
const isSubmitting = ref(false)
const scheduleStore = useScheduleStore()

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
    scheduleId?: string
    initialState?: ScheduleRequest
    communities: CommunityResponse[]
    alignActions?: 'left' | 'right'
  }>(),
  {
    alignActions: 'left'
  }
)

onMounted(() => {
  reset()
})

const reset = () => {
  now = new Date()
  formState.value.communityId = props.initialState?.communityId ?? ''
  formState.value.startsAt = props.initialState?.startsAt ?? resetStartTime.toISOString()
  formState.value.description = props.initialState?.description ?? ''
  formState.value.endsAt =
    props.initialState?.endsAt ??
    new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE).toISOString()
}

const toast = useToast()

const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  let response: ApiResponse<ScheduleRequest, never>
  if (props.scheduleId === '' || props.scheduleId === undefined) {
    response = await schedulesApi.post(formState.value)
  } else {
    response = await schedulesApi.put(props.scheduleId, formState.value)
  }
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return
  await scheduleStore.updateScheduleAsync(props.scheduleId ?? response.getCreatedId())
  showSuccessToast(toast, props.scheduleId ? 'Updated' : 'Created', 'Schedule')
  emits('afterSubmit')
  reset()
}

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
