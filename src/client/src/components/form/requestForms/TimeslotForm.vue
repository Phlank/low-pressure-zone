<template>
  <div class="timeslot-form">
    <FormArea align-actions="right">
      <IftaFormField
        input-id="startInput"
        label="Start"
        size="xs">
        <InputText
          id="startInput"
          :model-value="formatReadableTime(parseDate(formState.startsAt))"
          disabled />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('performanceType')"
        input-id="typeInput"
        label="Type"
        size="xs">
        <Select
          id="typeInput"
          v-model:model-value="formState.performanceType"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performanceType')"
          :options="performanceTypes"
          @change="() => validation.validateIfDirty('performanceType')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('name')"
        input-id="nameInput"
        label="Subtitle"
        optional
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="formState.name"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('name')" />
      </IftaFormField>
      <IftaFormField
        v-if="performerStore.performers.length > 0"
        :message="validation.message('performerId')"
        input-id="performerInput"
        label="Performer"
        size="m">
        <Select
          id="performerInput"
          v-model:model-value="formState.performerId"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performerId')"
          :options="performers"
          autofocus
          option-label="name"
          option-value="id"
          @change="() => validation.validateIfDirty('performerId')" />
      </IftaFormField>
      <IftaFormField
        v-if="performerStore.performers.length === 0"
        :message="validation.message('name')"
        input-id="performerNameInput"
        label="Performer Name"
        size="m">
        <InputText
          id="performerNameInput"
          v-model:model-value="formState.performerName"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('name')"
          autofocus />
      </IftaFormField>
      <IftaFormField
        v-if="performerStore.performers.length === 0"
        :message="validation.message('performerUrl')"
        input-id="performerUrlInput"
        label="Performer URL"
        size="m">
        <InputText
          id="performerUrlInput"
          v-model:model-value="formState.performerUrl"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performerUrl')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting || disabled"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { formatReadableTime, parseDate } from '@/utils/dateUtils'
import { performerRequestRules, timeslotRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { Button, InputText, Select, useToast } from 'primevue'
import { computed, onMounted, ref } from 'vue'
import timeslotsApi, {
  PerformanceType,
  performanceTypes,
  type TimeslotRequest
} from '@/api/resources/timeslotsApi.ts'
import performersApi, {
  type PerformerRequest,
  type PerformerResponse
} from '@/api/resources/performersApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { maximumLength, required } from '@/validation/rules/stringRules.ts'
import { applyRuleIf } from '@/validation/rules/untypedRules.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { err, ok, type Result } from '@/types/result.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import type { ValidationProblemDetails } from '@/api/apiResponse.ts'

const toast = useToast()
const performerStore = usePerformerStore()
const scheduleStore = useScheduleStore()

const props = defineProps<{
  scheduleId: string
  timeslotId?: string
  initialState: TimeslotRequest
  disabled: boolean
  performers: PerformerResponse[]
}>()

const defaultStartPerformerId = computed(() => {
  if (props.performers.length === 1) return props.performers[0].id
  return undefined
})

const formState = ref({
  startsAt: '',
  endsAt: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: '',
  performerName: '',
  performerUrl: ''
})
const timeslotRules = timeslotRequestRules(formState.value)
const performerRules = performerRequestRules
const validation = createFormValidation(formState, {
  startsAt: timeslotRules.startsAt,
  endsAt: timeslotRules.endsAt,
  performerId: applyRuleIf(timeslotRules.performerId, () => performerStore.performers.length > 0),
  performanceType: required(),
  name: maximumLength(64),
  performerName: applyRuleIf(performerRules.name, () => performerStore.performers.length === 0),
  performerUrl: applyRuleIf(performerRules.url, () => performerStore.performers.length === 0)
})

const isSubmitting = ref(false)
const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  let result: Result<null, null>
  if (props.timeslotId === '' || props.timeslotId === undefined) {
    result = await submitPost()
  } else {
    result = await submitPut()
  }
  isSubmitting.value = false
  if (result.isSuccess) {
    await scheduleStore.reloadTimeslotsAsync(props.scheduleId)
    reset()
    emits('afterSubmit')
  }
}

const submitPost = async (): Promise<Result<null, null>> => {
  if (formState.value.performerId === '') {
    const createPerformerResult = await createPerformer()
    if (!createPerformerResult.isSuccess) return err(null)
    formState.value.performerId = createPerformerResult.value!
  }
  const createTimeslotResponse = await timeslotsApi.post(props.scheduleId, formState.value)
  if (tryHandleUnsuccessfulResponse(createTimeslotResponse, toast, validation)) return err(null)
  showSuccessToast(toast, 'Created', 'Timeslot', formatReadableTime(formState.value.startsAt))
  return ok(null)
}

const createPerformer = async (): Promise<Result<string, null>> => {
  const request: PerformerRequest = {
    name: formState.value.performerName,
    url: formState.value.performerUrl
  }
  const response = await performersApi.post(request)
  if (!response.isSuccess()) {
    if (response.isInvalid()) {
      const details = response.error as ValidationProblemDetails<PerformerRequest>
      if (details.errors.name)
        validation.setValidity('performerName', { isValid: false, message: details.errors.name[0] })
      if (details.errors.url)
        validation.setValidity('performerUrl', { isValid: false, message: details.errors.url[0] })
    } else tryHandleUnsuccessfulResponse(response, toast)
    return err(null)
  }
  performerStore.add({
    id: response.getCreatedId(),
    name: formState.value.performerName,
    url: formState.value.performerUrl,
    isDeletable: true,
    isEditable: true,
    isLinkableToTimeslot: true
  })
  return ok(response.getCreatedId())
}

const submitPut = async (): Promise<Result<null, null>> => {
  if (props.timeslotId === '' || props.timeslotId === undefined) return err(null)
  console.log(props.timeslotId)
  const response = await timeslotsApi.put(props.scheduleId, props.timeslotId, formState.value)
  if (tryHandleUnsuccessfulResponse(response, toast, validation)) err(null)
  showSuccessToast(toast, 'Updated', 'Timeslot', formatReadableTime(formState.value.startsAt))
  return ok(null)
}

onMounted(async () => {
  reset()
  await performerStore.loadPerformersAsync()
})

const reset = () => {
  formState.value.startsAt = props.initialState.startsAt
  formState.value.endsAt = props.initialState.endsAt
  formState.value.performerId = defaultStartPerformerId.value ?? props.initialState.performerId
  formState.value.performanceType = props.initialState.performanceType
  formState.value.performerName = ''
  formState.value.performerUrl = ''
}

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
