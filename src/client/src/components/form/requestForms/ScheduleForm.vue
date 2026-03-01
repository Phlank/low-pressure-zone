<template>
  <div class="schedule-form">
    <FormArea>
      <IftaFormField
        :message="val.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="state.name"
          :disabled="isSubmitting"
          :invalid="!val.isValid('name')"
          @update:model-value="val.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('type')"
        input-id="typeInput"
        label="Type"
        size="m">
        <Select
          id="typeInput"
          v-model:model-value="state.type"
          :disabled="isSubmitting || props.schedule !== undefined"
          :invalid="!val.isValid('type')"
          :option-label="(data) => data.value"
          :option-value="(data) => data.key"
          :options="entriesToKeyValueArray(scheduleTypes)"
          autofocus
          @update:model-value="val.validateIfDirty('type')">
        </Select>
      </IftaFormField>
      <IftaFormField
        :message="val.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="m">
        <Select
          id="communityInput"
          v-model:model-value="state.communityId"
          :disabled="isSubmitting || props.schedule?.community.id !== undefined"
          :invalid="!val.isValid('communityId')"
          :options="availableCommunities"
          option-label="name"
          option-value="id"
          placeholder="Select an community"
          @update:model-value="val.validateIfDirty('communityId')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('startsAt')"
        input-id="startTimeInput"
        label="Start Time"
        size="m">
        <DatePicker
          id="startTimeInput"
          :disabled="state.startTime.getTime() < minStartTime.getTime() || isSubmitting"
          :invalid="!val.isValid('startsAt')"
          :min-date="minimumDate(state.startTime, minStartTime)"
          :model-value="state.startTime"
          fluid
          hourFormat="12"
          show-time
          @update:model-value="(value) => handleStartTimeChange(value as Date)" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('endsAt')"
        input-id="endTimeInput"
        label="End Time"
        size="m">
        <DatePicker
          id="endTimeInput"
          v-model:model-value="state.endTime"
          :disabled="state.endTime.getTime() < minStartTime.getTime() || isSubmitting"
          :invalid="!val.isValid('endsAt')"
          :max-date="new Date(state.startTime.getTime() + maxDurationMinutes * MS_PER_MINUTE)"
          :min-date="state.startTime"
          fluid
          hourFormat="12"
          show-time
          @update:model-value="val.validateIfDirty('endsAt')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('description')"
        input-id="descriptionInput"
        label="Description"
        optional
        size="xl">
        <Textarea
          id="descriptionInput"
          v-model:model-value="state.description"
          :disabled="isSubmitting"
          :invalid="!val.isValid('description')"
          auto-resize />
      </IftaFormField>
      <InlineFormField
        input-id="isOrganizersOnlyInput"
        label="Visible to Organizers Only"
        size="xs">
        <ToggleSwitch
          v-model="state.isOrganizersOnly"
          input-id="isOrganizersOnlyInput" />
      </InlineFormField>
    </FormArea>
    <MarkdownPreview
      :markdown-content="state.description"
      preview-title="Description Preview" />
  </div>
</template>

<script lang="ts" setup>
import { DatePicker, InputText, Select, Textarea, ToggleSwitch } from 'primevue'
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
import MarkdownPreview from '@/components/controls/MarkdownPreview.vue'

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

const { state, val, isSubmitting, submit, reset } = useEntityForm<
  ScheduleRequest,
  ScheduleFormState,
  ScheduleResponse
>({
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
  formState: state,
  validation: val,
  isSubmitting,
  reset,
  submit
})

const handleStartTimeChange = (value: Date) => {
  state.value.startTime = value
  if (state.value.endTime < state.value.startTime) {
    state.value.endTime = new Date(value.getTime() + defaultMinutes * MS_PER_MINUTE)
  } else if (
    state.value.endTime.getTime() >
    state.value.startTime.getTime() + maxDurationMinutes * MS_PER_MINUTE
  ) {
    state.value.endTime = new Date(value.getTime() + maxDurationMinutes * MS_PER_MINUTE)
  }
  val.validateIfDirty('startsAt')
}

const emit = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
