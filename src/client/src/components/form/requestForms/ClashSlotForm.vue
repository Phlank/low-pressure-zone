<template>
  <FormArea class="soundclash-form">
    <IftaFormField
      :message="val.message('startsAt')"
      input-id="startInput"
      label="Start Time"
      size="xs">
      <InputText
        :invalid="!val.isValid('startsAt')"
        :model-value="formatReadableTime(start)"
        disabled
        input-id="startInput"
        show-time />
    </IftaFormField>
    <IftaFormField
      input-id="durationInput"
      label="Duration"
      size="xs">
      <Select
        :options="['2 Hours']"
        default-value="2 Hours"
        disabled
        input-id="durationInput" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('performerOneId')"
      input-id="performerOneInput"
      label="Performer One"
      size="m">
      <Select
        v-model="state.performerOneId"
        :invalid="!val.isValid('performerOneId')"
        :options="performers.performers"
        input-id="performerOneInput"
        option-label="name"
        option-value="id"
        placeholder="Select Performer One"
        @update:model-value="val.validateIfDirty('performerOneId')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('performerTwoId')"
      input-id="performerTwoInput"
      label="Performer Two"
      size="m">
      <Select
        v-model="state.performerTwoId"
        :invalid="!val.isValid('performerTwoId')"
        :options="performers.performers"
        input-id="performerTwoInput"
        option-label="name"
        option-value="id"
        placeholder="Select Performer Two"
        @update:model-value="val.validateIfDirty('performerTwoId')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('rounds')"
      input-id="roundOneInput"
      label="Round One"
      size="xl"
      style="width: 100%">
      <InputText
        v-model="state.rounds[0]"
        :invalid="!val.isValid('rounds')"
        @update:model-value="val.validateIfDirty('rounds')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('rounds')"
      input-id="roundTwoInput"
      label="Round Two"
      size="m">
      <InputText
        v-model="state.rounds[1]"
        :invalid="!val.isValid('rounds')"
        @update:model-value="val.validateIfDirty('rounds')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('rounds')"
      input-id="roundThreeInput"
      label="Round Three"
      size="m">
      <InputText
        v-model="state.rounds[2]"
        :invalid="!val.isValid('rounds')"
        @update:model-value="val.validateIfDirty('rounds')" />
    </IftaFormField>
  </FormArea>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useEntityForm } from '@/composables/useEntityForm.ts'
import type { ClashSlotRequest, ClashSlotResponse } from '@/api/resources/clashSlotsApi.ts'
import { computed, ref, type Ref } from 'vue'
import { formatReadableTime, parseDate } from '@/utils/dateUtils.ts'
import { clashSlotRequestRules } from '@/validation/requestRules.ts'
import { alwaysValid } from '@/validation/rules/untypedRules.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { InputText, Select } from 'openvue'

const schedules = useScheduleStore()
const performers = usePerformerStore()

const props = defineProps<{
  clashSlot?: ClashSlotResponse
  scheduleId: string
  start: Date
}>()

type SoundclashFormState = ClashSlotRequest & {
  startTime: Date
}

const formStateInitializeFn = (entity?: ClashSlotResponse) => {
  const state: Ref<SoundclashFormState> = ref({
    scheduleId: props.scheduleId,
    startsAt: props.start.toISOString(),
    startTime: computed({
      get: () => parseDate(state.value.startsAt),
      set: (value: Date) => {
        state.value.startsAt = value.toISOString()
      }
    }),
    duration: entity?.duration ?? 2,
    performerOneId: entity?.performerOneId ?? '',
    performerTwoId: entity?.performerTwoId ?? '',
    rounds: entity?.rounds ?? ['', '', '']
  })
  return state
}

const { state, val, isSubmitting, submit, reset } = useEntityForm<
  ClashSlotRequest,
  SoundclashFormState,
  ClashSlotResponse
>({
  entity: props.clashSlot,
  formStateInitializeFn: (entity) => formStateInitializeFn(entity),
  validationRules: {
    ...clashSlotRequestRules,
    startTime: alwaysValid()
  },
  createPersistentEntityFn: schedules.createClashSlot(props.scheduleId),
  updatePersistentEntityFn: schedules.updateClashSlot(props.scheduleId),
  onSubmitted: () => {
    emit('submitted')
  }
})

const emit = defineEmits<{
  submitted: []
}>()

defineExpose({
  isSubmitting,
  submit,
  reset
})
</script>
