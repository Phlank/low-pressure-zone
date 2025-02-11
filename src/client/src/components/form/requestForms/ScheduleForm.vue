<template>
  <div class="schedule-form desktop-inline">
    <IftaLabel class="input input--small">
      <Select
        class="input__field"
        id="audienceSelect"
        :options="audiences"
        option-label="name"
        option-value="id"
        :disabled="disabled"
        placeholder="Select an audience"
        :invalid="!validation.isValid('audienceId')"
        :model-value="formState.audienceId"
        @update:model-value="(value) => (formState.audienceId = value)"
      />
      <ValidationLabel for="audienceSelect" :message="validation.message('audienceId')">
        Audience
      </ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input input--small">
      <DatePicker
        class="input__field"
        id="startTime"
        hourFormat="12"
        :min-date="new Date()"
        :model-value="formState.startTime"
        @update:model-value="handleStartUpdate"
        :invalid="!validation.isValid('start')"
        show-time
        fluid
      />
      <ValidationLabel for="startTime" :message="validation.message('start')">
        Start Time
      </ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input input--small">
      <DatePicker
        class="input__field"
        id="startTime"
        hourFormat="12"
        :min-date="formState.startTime"
        :max-date="new Date(formState.startTime.getTime() + MAX_DURATION_MINUTES * 60000)"
        :model-value="formState.endTime"
        @update:model-value="handleEndUpdate"
        :invalid="!validation.isValid('end')"
        show-time
        fluid
      />
      <ValidationLabel for="endTime" :message="validation.message('end')">
        End Time
      </ValidationLabel>
    </IftaLabel>
  </div>
</template>

<script lang="ts" setup>
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import { DatePicker, IftaLabel, Select } from 'primevue'
import ValidationLabel from '../ValidationLabel.vue'
import { reactive } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation'
import type { ScheduleRequest } from '@/api/schedules/scheduleRequest'
import { audienceValidator } from '@/validation/rules/composed/scheduleValidators'
import { alwaysValid } from '@/validation/rules/single/untypedRules'

const MAX_DURATION_MINUTES = 1440
let now = new Date()

const formState = reactive({
  audienceId: '',
  startTime: now,
  start: now.toISOString(),
  endTime: new Date(now.getTime() + 60 * 60000),
  end: now.toISOString()
})
const validation = createFormValidation(formState as ScheduleRequest, {
  audienceId: audienceValidator,
  start: alwaysValid,
  end: alwaysValid
})

defineProps<{
  audiences: AudienceResponse[]
  disabled: boolean
}>()

const handleStartUpdate = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  const duration = formState.endTime.getTime() - formState.startTime.getTime()
  formState.startTime = value as Date
  formState.start = formState.startTime.toISOString()
  formState.endTime = new Date(formState.startTime.getTime() + duration)
  formState.end = formState.endTime.toISOString()
}

const handleEndUpdate = (value: Date | Date[] | (Date | null)[] | null | undefined) => {
  formState.endTime = value as Date
  formState.end = formState.endTime.toISOString()
}

const reset = () => {
  now = new Date()
  formState.audienceId = ''
  formState.startTime = now
  formState.start = now.toISOString()
  formState.endTime = new Date(now.getTime() + 60 * 60000)
  formState.end = formState.endTime.toISOString()
}

defineExpose({ formState, validation, reset })
</script>
