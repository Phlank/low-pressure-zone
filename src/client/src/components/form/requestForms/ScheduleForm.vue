<template>
  <div class="schedule-form">
    <FormArea>
      <IftaFormField
        :message="validation.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="formState.name"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('name')"
          @update:model-value="validation.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('type')"
        input-id="typeInput"
        label="Type"
        size="m">
        <Select
          id="typeInput"
          v-model:model-value="formState.type"
          :disabled="isSubmitting || props.schedule !== undefined"
          :invalid="!validation.isValid('type')"
          :option-label="(data) => data.value"
          :option-value="(data) => data.key"
          :options="entriesToKeyValueArray(scheduleTypes)"
          autofocus
          @update:model-value="validation.validateIfDirty('type')">
        </Select>
      </IftaFormField>
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
          option-label="name"
          option-value="id"
          placeholder="Select an community"
          @update:model-value="validation.validateIfDirty('communityId')" />
      </IftaFormField>
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
      <InlineFormField
        label="Visible to Organizers Only"
        input-id="isOrganizersOnlyInput"
        size="xs">
        <ToggleSwitch
          input-id="isOrganizersOnlyInput"
          v-model="formState.isOrganizersOnly" />
      </InlineFormField>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { DatePicker, Select, Textarea, InputText, ToggleSwitch } from 'primevue'
import { computed, onMounted, type Ref, ref } from 'vue'
import { MS_PER_MINUTE } from '@/constants/times'
import { minimumDate, parseDate } from '@/utils/dateUtils'
import { scheduleRequestRules } from '@/validation/requestRules'
import { type ScheduleRequest, type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { useEntityForm } from '@/composables/useEntityForm.ts'
import { alwaysValid } from '@/validation/rules/untypedRules.ts'
import entriesToKeyValueArray from '@/utils/entriesToKeyValueArray.ts'
import { scheduleTypes } from '@/constants/scheduleTypes.ts'
import InlineFormField from '@/components/form/InlineFormField.vue'

const maxDurationMinutes = 1440
const defaultMinutes = 60
const schedules = useScheduleStore()

const now = new Date()
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

interface ScheduleFormState extends ScheduleRequest {
  startTime: Date
  endTime: Date
}

const {
  state: formState,
  val: validation,
  isSubmitting,
  submit,
  reset
} = useEntityForm<ScheduleRequest, ScheduleFormState, ScheduleResponse>({
  validationRules: (form) => ({
    startTime: alwaysValid(),
    endTime: alwaysValid(),
    ...scheduleRequestRules(form)
  }),
  entity: props.schedule,
  formStateInitializeFn: (schedule) => {
    const stateRef: Ref<ScheduleFormState> = ref({
      type: schedule?.type ?? scheduleTypes.Hourly,
      communityId: schedule?.community.id ?? '',
      startTime: computed({
        get: () => parseDate(stateRef.value.startsAt),
        set: (value) => {
          stateRef.value.startsAt = value.toISOString()
        }
      }),
      startsAt: schedule?.startsAt ?? resetStartTime.toISOString(),
      name: schedule?.name ?? '',
      description: schedule?.description ?? '',
      endTime: computed({
        get: () => parseDate(stateRef.value.endsAt),
        set: (value) => {
          stateRef.value.endsAt = value.toISOString()
        }
      }),
      endsAt:
        schedule?.endsAt ??
        new Date(resetStartTime.getTime() + defaultMinutes * MS_PER_MINUTE).toISOString(),
      isOrganizersOnly: schedule?.isOrganizersOnly ?? false
    })
    return stateRef
  },
  createPersistentEntityFn: schedules.createSchedule,
  updatePersistentEntityFn: schedules.updateSchedule,
  onSubmitted: () => emit('submitted')
})

defineExpose({
  formState,
  validation,
  isSubmitting,
  reset,
  submit
})

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

const emit = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
