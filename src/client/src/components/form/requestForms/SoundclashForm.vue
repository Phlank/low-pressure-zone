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
      :message="val.message('roundOne')"
      input-id="roundOneInput"
      label="Round One"
      size="m">
      <InputText
        v-model="state.roundOne"
        :invalid="!val.isValid('roundOne')"
        @update:model-value="val.validateIfDirty('roundOne')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('roundTwo')"
      input-id="roundTwoInput"
      label="Round Two"
      size="m">
      <InputText
        v-model="state.roundTwo"
        :invalid="!val.isValid('roundTwo')"
        @update:model-value="val.validateIfDirty('roundTwo')" />
    </IftaFormField>
    <IftaFormField
      :message="val.message('roundThree')"
      input-id="roundThreeInput"
      label="Round Three"
      size="m">
      <InputText
        v-model="state.roundThree"
        :invalid="!val.isValid('roundThree')"
        @update:model-value="val.validateIfDirty('roundThree')" />
    </IftaFormField>
  </FormArea>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useEntityForm } from '@/composables/useEntityForm.ts'
import type { SoundclashRequest, SoundclashResponse } from '@/api/resources/soundclashApi.ts'
import { computed, ref, type Ref } from 'vue'
import { formatReadableTime, parseDate } from '@/utils/dateUtils.ts'
import { addHours } from 'date-fns'
import { soundclashRequestRules } from '@/validation/requestRules.ts'
import { alwaysValid } from '@/validation/rules/untypedRules.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { InputText, Select } from 'primevue'

const schedules = useScheduleStore()
const performers = usePerformerStore()

const props = defineProps<{
  soundclash?: SoundclashResponse
  scheduleId: string
  start: Date
}>()

type SoundclashFormState = SoundclashRequest & {
  startTime: Date
  endTime: Date
}

const formStateInitializeFn = (entity?: SoundclashResponse) => {
  const state: Ref<SoundclashFormState> = ref({
    scheduleId: props.scheduleId,
    startsAt: props.start.toISOString(),
    startTime: computed({
      get: () => parseDate(state.value.startsAt),
      set: (value: Date) => {
        state.value.startsAt = value.toISOString()
      }
    }),
    endsAt: addHours(props.start, 2).toISOString(),
    endTime: computed({
      get: () => parseDate(state.value.endsAt),
      set: (value: Date) => {
        state.value.endsAt = value.toISOString()
      }
    }),
    performerOneId: entity?.performerOne.id ?? '',
    performerTwoId: entity?.performerTwo.id ?? '',
    roundOne: entity?.roundOne ?? '',
    roundTwo: entity?.roundTwo ?? '',
    roundThree: entity?.roundThree ?? ''
  })
  return state
}

const { state, val, isSubmitting, submit, reset } = useEntityForm<
  SoundclashRequest,
  SoundclashFormState,
  SoundclashResponse
>({
  entity: props.soundclash,
  formStateInitializeFn: (entity) => formStateInitializeFn(entity),
  validationRules: (entity) => ({
    ...soundclashRequestRules(entity),
    startTime: alwaysValid(),
    endTime: alwaysValid()
  }),
  createPersistentEntityFn: schedules.createSoundclash,
  updatePersistentEntityFn: schedules.updateSoundclash,
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
