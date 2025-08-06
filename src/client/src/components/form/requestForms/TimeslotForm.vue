<template>
  <div class="timeslot-form">
    <FormArea>
      <IftaFormField
        input-id="startInput"
        label="Start"
        size="m">
        <InputText
          id="startInput"
          :model-value="formatReadableTime(parseDate(formState.startsAt))"
          disabled />
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
          v-model:model-value="formState.name"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('name')"
          autofocus />
      </IftaFormField>
      <IftaFormField
        v-if="performerStore.performers.length === 0"
        :message="validation.message('url')"
        input-id="performerUrlInput"
        label="Performer URL"
        size="m">
        <InputText
          id="performerUrlInput"
          v-model:model-value="formState.url"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('url')" />
      </IftaFormField>
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
          class="input__field"
          @change="() => validation.validateIfDirty('performanceType')" />
      </IftaFormField>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { formatReadableTime, parseDate } from '@/utils/dateUtils'
import { performerRequestRules, timeslotRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { InputText, Select, useToast } from 'primevue'
import { computed, onMounted, type Ref, ref } from 'vue'
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
import { required } from '@/validation/rules/stringRules.ts'
import { applyRuleIf } from '@/validation/rules/untypedRules.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { err, ok, type Result } from '@/types/result.ts'
import { showCreateSuccessToast } from '@/utils/toastUtils.ts'

const toast = useToast()
const performerStore = usePerformerStore()

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

const formState: Ref<TimeslotRequest & PerformerRequest> = ref({
  startsAt: '',
  endsAt: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: '',
  url: ''
})
const timeslotRules = timeslotRequestRules(formState.value)
const performerRules = performerRequestRules
const validation = createFormValidation(formState, {
  startsAt: timeslotRules.startsAt,
  endsAt: timeslotRules.endsAt,
  performerId: applyRuleIf(timeslotRules.performerId, () => performerStore.performers.length > 0),
  performanceType: required(),
  name: applyRuleIf(performerRules.name, () => performerStore.performers.length === 0),
  url: applyRuleIf(performerRules.url, () => performerStore.performers.length === 0)
})

const isSubmitting = ref(false)
const submit = async () => {
  if (!validation.validate()) return
  if (props.timeslotId === undefined) {
    await submitPost()
    return
  }
  await submitPut()
}
const submitPost = async () => {
  if (formState.value.performerId === '') {
    const createPerformerResult = await createPerformer()
    if (!createPerformerResult.isSuccess) return
    formState.value.performerId = createPerformerResult.value!
  }
  const createTimeslotResponse = await timeslotsApi.post(props.scheduleId, formState.value)
  if (tryHandleUnsuccessfulResponse(createTimeslotResponse, toast, validation)) return
  showCreateSuccessToast(toast, 'Timeslot', formState.value.startsAt)
  reset()
}
const createPerformer = async (): Promise<Result<string, null>> => {
  const response = await performersApi.post(formState.value)
  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err(null)
  performerStore.add({
    id: response.getCreatedId(),
    name: formState.value.name,
    url: formState.value.url,
    isDeletable: true,
    isEditable: true,
    isLinkableToTimeslot: true
  })
  return ok(response.getCreatedId())
}
const submitPut = async () => {}

onMounted(async () => {
  reset()
  await performerStore.loadPerformersAsync()
})

const reset = () => {
  formState.value.startsAt = props.initialState.startsAt
  formState.value.endsAt = props.initialState.endsAt
  formState.value.performerId = defaultStartPerformerId.value ?? props.initialState.performerId
  formState.value.performanceType = props.initialState.performanceType
  formState.value.name = ''
  formState.value.url = ''
}

defineExpose({ formState, validation, reset, submit })
</script>
