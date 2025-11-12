<template>
  <div class="timeslot-form">
    <FormArea align-actions="right">
      <IftaFormField
        :message="validation.message('performanceType')"
        input-id="typeInput"
        label="Type"
        size="m">
        <Select
          id="typeInput"
          v-model:model-value="formState.performanceType"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performanceType')"
          :options="performanceTypes"
          @change="() => validation.validateIfDirty('performanceType')" />
      </IftaFormField>
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
        label="Duration"
        input-id="durationInput"
        size="xs">
        <Select
          id="durationInput"
          :options="durationOptions"
          option-label="label"
          option-value="value"
          v-model:model-value="formState.duration">
        </Select>
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
        :message="validation.message('performerName')"
        input-id="performerNameInput"
        label="Performer Name"
        size="m">
        <InputText
          id="performerNameInput"
          v-model:model-value="formState.performerName"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performerName')"
          autofocus />
      </IftaFormField>
      <IftaFormField
        v-if="performerStore.performers.length === 0"
        :message="validation.message('performerUrl')"
        input-id="performerUrlInput"
        label="Performer URL"
        optional
        size="m">
        <InputText
          id="performerUrlInput"
          v-model:model-value="formState.performerUrl"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performerUrl')" />
      </IftaFormField>
      <FormField
        v-if="formState.performanceType === 'Prerecorded DJ Set'"
        input-id="fileInput"
        label="Upload File"
        :message="validation.message('file')"
        size="m">
        <FileUpload
          mode="basic"
          accept="media/*,audio/*"
          @select="onFileSelect"
          @remove="onFileRemove" />
      </FormField>
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
import { formatDurationOption, formatReadableTime, parseDate, parseTime } from '@/utils/dateUtils'
import { performerRequestRules, timeslotRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import {
  Button,
  InputText,
  Select,
  useToast,
  FileUpload,
  type FileUploadSelectEvent,
  type FileUploadRemoveEvent
} from 'primevue'
import { computed, onMounted, ref, watch } from 'vue'
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
import { alwaysValid, applyRuleIf } from '@/validation/rules/untypedRules.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { err, ok, type Result } from '@/types/result.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import type { ValidationProblemDetails } from '@/api/apiResponse.ts'
import FormField from '@/components/form/FormField.vue'

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

type TimeslotFormState = TimeslotRequest & {
  duration: number
  performerName: string
  performerUrl: string
}

const formState = ref<TimeslotFormState>({
  startsAt: '',
  duration: 60,
  endsAt: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: '',
  file: undefined,
  performerName: '',
  performerUrl: ''
})

const timeslotRules = timeslotRequestRules(formState.value)
const performerRules = performerRequestRules
const validation = createFormValidation(formState, {
  startsAt: timeslotRules.startsAt,
  duration: alwaysValid(),
  endsAt: timeslotRules.endsAt,
  performerId: applyRuleIf(timeslotRules.performerId, () => performerStore.performers.length > 0),
  performanceType: timeslotRules.performanceType,
  name: timeslotRules.name,
  file: timeslotRules.file,
  performerName: applyRuleIf(performerRules.name, () => performerStore.performers.length === 0),
  performerUrl: applyRuleIf(performerRules.url, () => performerStore.performers.length === 0)
})

const onFileSelect = (e: FileUploadSelectEvent) => {
  formState.value.file = e.files.length > 0 ? (e.files[0] as File) : undefined
}

const onFileRemove = (e: FileUploadRemoveEvent) => {
  if (e.files.length === 0)
    formState.value.file = undefined
}

const isSubmitting = ref(false)

const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true

  let result: Result<null, string>
  if (props.timeslotId === '' || props.timeslotId === undefined) {
    result = await submitPost()
  } else {
    result = await submitPut()
  }
  isSubmitting.value = false

  if (!result.isSuccess) return

  await scheduleStore.reloadTimeslotsAsync(props.scheduleId)
  reset()
  emits('afterSubmit')
}

const submitPost = async (): Promise<Result<null, string>> => {
  if (props.timeslotId !== '' && props.timeslotId !== undefined)
    return err('Cannot POST when timeslotId is provided')

  if (formState.value.performerId === '') {
    const createPerformerResult = await createPerformer()
    if (!createPerformerResult.isSuccess) return err('Failed to create performer')

    formState.value.performerId = createPerformerResult.value!
  }

  const createTimeslotResponse = await timeslotsApi.post(props.scheduleId, formState.value)
  if (tryHandleUnsuccessfulResponse(createTimeslotResponse, toast, validation))
    return err('API failure when creating timeslot')

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

const submitPut = async (): Promise<Result<null, string>> => {
  if (props.timeslotId === '' || props.timeslotId === undefined)
    return err('Cannot PUT when timeslotId is not provided')

  const response = await timeslotsApi.put(props.scheduleId, props.timeslotId, formState.value)
  if (tryHandleUnsuccessfulResponse(response, toast, validation))
    return err('API failure when updating timeslot')

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
  formState.value.duration = Math.round(
    (parseTime(props.initialState.endsAt) - parseTime(props.initialState.startsAt)) / 60000
  )
  formState.value.performerId = defaultStartPerformerId.value ?? props.initialState.performerId
  formState.value.performanceType = props.initialState.performanceType
  formState.value.name = props.initialState.name
  formState.value.performerName = ''
  formState.value.performerUrl = ''
}

const durationOptions = computed((): { label: string; value: number }[] => {
  const options: { label: string; value: number }[] = []
  const schedule = scheduleStore.schedules.find((schedule) => schedule.id === props.scheduleId)
  if (!schedule) return []

  const endOfSchedule = parseTime(schedule.endsAt)

  const timeslots = schedule.timeslots
  const timeslotsFollowingCurrent = timeslots
    .filter((timeslot) => parseTime(timeslot.startsAt) > parseTime(formState.value.startsAt))
    .sort((a, b) => parseTime(a.startsAt) - parseTime(b.startsAt))
  const nextBoundaryTime =
    timeslotsFollowingCurrent.length > 0
      ? parseTime(timeslotsFollowingCurrent[0].startsAt)
      : endOfSchedule

  const maxDurationMinutes = Math.floor(
    (nextBoundaryTime - parseTime(formState.value.startsAt)) / 60000
  )
  if (maxDurationMinutes >= 60)
    options.push({
      label: formatDurationOption(60),
      value: 60
    })
  if (maxDurationMinutes >= 120)
    options.push({
      label: formatDurationOption(120),
      value: 120
    })
  if (maxDurationMinutes >= 180)
    options.push({
      label: formatDurationOption(180),
      value: 180
    })

  return options
})

watch(
  () => formState.value.duration,
  (newDuration) => {
    const startDate = parseDate(formState.value.startsAt)
    const endDate = new Date(startDate.getTime() + newDuration * 60000)
    formState.value.endsAt = endDate.toISOString()
  }
)

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
