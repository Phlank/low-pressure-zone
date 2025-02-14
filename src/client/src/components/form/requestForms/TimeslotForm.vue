<template>
  <div class="timeslot-form">
    <div class="desktop-inline">
      <IftaLabel class="input input--small">
        <DatePicker
          class="input__field"
          id="startInput"
          :model-value="parseDate(formState.start)"
          disabled
        />
        <label for="startInput">Start</label>
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Select
          class="input__field"
          id="timeslotPerformerSelect"
          option-label="name"
          option-value="id"
          :disabled="isSubmitting || disabled"
          :options="performers"
          v-model:model-value="formState.performerId"
          @change="() => validation.validateIfDirty('performerId')"
          :invalid="!validation.isValid('performerId')"
        />
        <ValidationLabel
          for="timeslotPerformerSelect"
          text="Performer"
          :message="validation.message('performerId')"
        />
      </IftaLabel>
      <IftaLabel class="input input--small">
        <Select
          class="input__field"
          id="timeslotTypeSelect"
          :disabled="isSubmitting || disabled"
          :options="performanceTypes"
          v-model:model-value="formState.performanceType"
          @change="() => validation.validateIfDirty('performanceType')"
          :invalid="!validation.isValid('performanceType')"
        />
        <ValidationLabel
          for="timeslotTypeSelect"
          text="Type"
          :message="validation.message('performanceType')"
        />
      </IftaLabel>
    </div>
    <div class="desktop-inline">
      <IftaLabel class="input input--medium">
        <InputText
          class="input__field"
          id="timeslotNameInput"
          :disabled="isSubmitting || disabled"
          v-model:model-value="formState.name"
          @change="() => validation.validateIfDirty('name')"
          :invalid="!validation.isValid('name')"
        />
        <ValidationLabel
          for="timeslotNameInput"
          text="Name"
          :message="validation.message('name')"
          optional
        />
      </IftaLabel>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { PerformerResponse } from '@/api/performers/performerResponse'
import type { TimeslotRequest } from '@/api/schedules/timeslots/timeslotRequest'
import { parseDate } from '@/utils/dateUtils'
import { timeslotRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, Select } from 'primevue'
import { reactive, ref } from 'vue'
import ValidationLabel from '../ValidationLabel.vue'
import { performanceTypes } from '@/api/schedules/timeslots/performanceType'

const props = defineProps<{
  initialState: TimeslotRequest
  disabled: boolean
  performers: PerformerResponse[]
}>()

const formState: TimeslotRequest = reactive({
  start: props.initialState.start,
  end: props.initialState.end,
  performerId: props.initialState.performerId,
  performanceType: props.initialState.performanceType,
  name: props.initialState.name
})
const validation = createFormValidation(formState, timeslotRequestRules(formState))

const isSubmitting = ref(false)

const reset = () => {
  formState.start = props.initialState.start
  formState.end = props.initialState.end
  formState.performerId = props.initialState.performerId
  formState.performanceType = props.initialState.performanceType
  formState.name = props.initialState.end
}

defineExpose({ formState, validation, reset })
</script>
