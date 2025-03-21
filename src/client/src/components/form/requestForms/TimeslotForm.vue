<template>
  <div class="timeslot-form">
    <div class="desktop-inline">
      <IftaLabel class="input input--small">
        <InputText
          id="startInput"
          :model-value="formatTimeslot(parseDate(formState.startsAt))"
          class="input__field"
          disabled />
        <label for="startInput">Start</label>
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Select
          id="timeslotPerformerSelect"
          v-model:model-value="formState.performerId"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performerId')"
          :options="performers"
          class="input__field"
          option-label="name"
          option-value="id"
          @change="() => validation.validateIfDirty('performerId')" />
        <ValidationLabel
          :message="validation.message('performerId')"
          for="timeslotPerformerSelect"
          text="Performer" />
      </IftaLabel>
      <IftaLabel class="input input--small">
        <Select
          id="timeslotTypeSelect"
          v-model:model-value="formState.performanceType"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('performanceType')"
          :options="performanceTypes"
          class="input__field"
          @change="() => validation.validateIfDirty('performanceType')" />
        <ValidationLabel
          :message="validation.message('performanceType')"
          for="timeslotTypeSelect"
          text="Type" />
      </IftaLabel>
    </div>
    <div class="desktop-inline">
      <IftaLabel class="input input--medium">
        <InputText
          id="timeslotNameInput"
          v-model:model-value="formState.name"
          :disabled="isSubmitting || disabled"
          :invalid="!validation.isValid('name')"
          class="input__field"
          @change="() => validation.validateIfDirty('name')" />
        <ValidationLabel
          :message="validation.message('name')"
          for="timeslotNameInput"
          optional
          text="Name" />
      </IftaLabel>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { formatTimeslot, parseDate } from '@/utils/dateUtils'
import { timeslotRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText, Select } from 'primevue'
import { computed, onMounted, reactive, ref } from 'vue'
import ValidationLabel from '../ValidationLabel.vue'
import {
  PerformanceType,
  performanceTypes,
  type TimeslotRequest
} from '@/api/resources/timeslotsApi.ts'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'

const props = defineProps<{
  initialState: TimeslotRequest
  disabled: boolean
  performers: PerformerResponse[]
}>()

const defaultStartPerformerId = computed(() => {
  if (props.performers.length === 1) return props.performers[0].id
  return undefined
})

const formState: TimeslotRequest = reactive({
  startsAt: '',
  endsAt: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: null
})
const validation = createFormValidation(formState, timeslotRequestRules(formState))

const isSubmitting = ref(false)

onMounted(() => {
  reset()
})

const reset = () => {
  formState.startsAt = props.initialState.startsAt
  formState.endsAt = props.initialState.endsAt
  formState.performerId = defaultStartPerformerId.value ?? props.initialState.performerId
  formState.performanceType = props.initialState.performanceType
  formState.name = props.initialState.name
}

defineExpose({ formState, validation, reset })
</script>
