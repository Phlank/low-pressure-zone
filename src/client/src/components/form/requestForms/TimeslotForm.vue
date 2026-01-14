<template>
  <div class="timeslot-form">
    <FormArea align-actions="right">
      <IftaFormField
        :message="timeslotForm.val.message('performanceType')"
        input-id="typeInput"
        label="Type"
        size="m">
        <Select
          id="typeInput"
          v-model:model-value="timeslotForm.state.value.performanceType"
          :disabled="timeslot !== undefined || isSubmitting"
          :invalid="!timeslotForm.val.isValid('performanceType')"
          :options="performanceTypes"
          autofocus
          @change="() => timeslotForm.val.validateIfDirty('performanceType')" />
      </IftaFormField>
      <IftaFormField
        input-id="startInput"
        label="Start"
        size="xs">
        <InputText
          id="startInput"
          :model-value="formatReadableTime(parseDate(timeslotForm.state.value.startsAt))"
          disabled />
      </IftaFormField>
      <IftaFormField
        input-id="durationInput"
        label="Duration"
        size="xs">
        <Select
          id="durationInput"
          v-model:model-value="timeslotForm.state.value.duration"
          :disabled="
            isSubmitting ||
            (timeslot &&
              timeslotForm.state.value.performanceType === 'Prerecorded DJ Set' &&
              !timeslotForm.state.value.replaceMedia)
          "
          :options="durationOptions"
          option-label="label"
          option-value="value">
        </Select>
      </IftaFormField>
      <IftaFormField
        :message="timeslotForm.val.message('subtitle')"
        input-id="subtitleInput"
        label="Subtitle"
        optional
        size="m">
        <InputText
          id="subtitleInput"
          v-model:model-value="timeslotForm.state.value.subtitle"
          :disabled="isSubmitting"
          :invalid="!timeslotForm.val.isValid('subtitle')" />
      </IftaFormField>
      <IftaFormField
        v-if="performers.performers.length > 0"
        :message="timeslotForm.val.message('performerId')"
        input-id="performerInput"
        label="Performer"
        size="m">
        <Select
          id="performerInput"
          v-model:model-value="timeslotForm.state.value.performerId"
          :disabled="isSubmitting"
          :invalid="!timeslotForm.val.isValid('performerId')"
          :options="performers.linkablePerformers"
          autofocus
          option-label="name"
          option-value="id"
          @change="() => timeslotForm.val.validateIfDirty('performerId')" />
      </IftaFormField>
      <IftaFormField
        v-if="performers.performers.length === 0"
        :message="performerForm.val.message('name')"
        input-id="performerNameInput"
        label="Performer Name"
        size="m">
        <InputText
          id="performerNameInput"
          v-model:model-value="performerForm.state.value.name"
          :disabled="isSubmitting"
          :invalid="!performerForm.val.isValid('name')"
          autofocus />
      </IftaFormField>
      <IftaFormField
        v-if="performers.performers.length === 0"
        :message="performerForm.val.message('url')"
        input-id="performerUrlInput"
        label="Performer URL"
        optional
        size="m">
        <InputText
          id="performerUrlInput"
          v-model:model-value="performerForm.state.value.url"
          :disabled="isSubmitting"
          :invalid="!performerForm.val.isValid('url')" />
      </IftaFormField>
      <FormField
        v-if="timeslotForm.state.value.performanceType === 'Prerecorded DJ Set'"
        :message="timeslotForm.val.message('file')"
        class="timeslot-form__file-upload"
        input-id="fileInput"
        label="Upload Mix"
        size="m">
        <div class="timeslot-form__file-upload__help-text">
          <div>File limits:</div>
          <ul>
            <li>No larger than 1 GB.</li>
            <li>
              Accepted encodings: wav, flac, m4a/mp4, ogg+opus, ogg+vorbis. All non-mp3 files are
              re-encoded to 320kbps mp3.
            </li>
            <li>
              Lossless files must be at least 16-bit, 44.1kHz. Lossy files must have a bitrate of at
              least 256kbps.
            </li>
            <li>Duration must match the timeslot +/- 2 minutes.</li>
          </ul>
        </div>
        <div
          v-if="timeslot"
          class="checkbox-area">
          <div class="checkbox-area__item">
            <Checkbox
              id="replaceMediaInput"
              v-model="timeslotForm.state.value.replaceMedia"
              binary />
            <label for="replaceMediaInput">Replace uploaded file</label>
          </div>
        </div>
        <FileUpload
          :disabled="
            isSubmitting ||
            (timeslot &&
              !timeslotForm.state.value.replaceMedia &&
              timeslotForm.state.value.performanceType == PerformanceType.Prerecorded)
          "
          mode="basic"
          @remove="onFileRemove"
          @select="onFileSelect">
          <template #filelabel>
            <span v-if="timeslotForm.state.value.file">{{
              timeslotForm.state.value.file.name
            }}</span>
            <span v-else-if="timeslot && !timeslotForm.state.value.replaceMedia">{{
              timeslot?.uploadedFileName
            }}</span>
            <span v-else>No file chosen</span>
          </template>
        </FileUpload>
        <ProgressBar
          v-if="timeslotForm.state.value.file !== null && isSubmitting"
          :value="Math.floor(timeslotForm.progress.value * 100)"
          style="width: 100%">
          {{ uploadProgressText }}
        </ProgressBar>
      </FormField>
      <FormField
        v-if="timeslotForm.state.value.performanceType === 'Prerecorded DJ Set'"
        size="m">
        <Message v-if="timeslotForm.state.value.performanceType === 'Prerecorded DJ Set'">
          Uploading prerecorded sets is a new feature, and there may be some issues to work out
          still. Please let Phlank know if you have any problems!
        </Message>
      </FormField>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { formatDurationOption, formatReadableTime, parseDate, parseTime } from '@/utils/dateUtils'
import { performerRequestRules, timeslotRequestRules } from '@/validation/requestRules'
import {
  Checkbox,
  FileUpload,
  type FileUploadRemoveEvent,
  type FileUploadSelectEvent,
  InputText,
  Message,
  ProgressBar,
  Select
} from 'primevue'
import { computed, onMounted, ref, watch } from 'vue'
import {
  PerformanceType,
  performanceTypes,
  type TimeslotRequest,
  type TimeslotResponse
} from '@/api/resources/timeslotsApi.ts'
import { type PerformerRequest, type PerformerResponse } from '@/api/resources/performersApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { alwaysValid, required } from '@/validation/rules/untypedRules.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import FormField from '@/components/form/FormField.vue'
import { useEntityForm } from '@/composables/useEntityForm.ts'
import { addHours } from 'date-fns'
import { withinRange } from '@/validation/rules/numberRules.ts'
import { combineRules } from '@/validation/types/validationRule.ts'
import { err, ok, type Result } from '@/types/result.ts'

const performers = usePerformerStore()
const schedules = useScheduleStore()

const props = defineProps<{
  scheduleId: string
  timeslot?: TimeslotResponse
  start: Date
}>()

const defaultStartPerformerId = computed(() => {
  if (performers.linkablePerformers.length === 1) return performers.linkablePerformers[0]!.id
  return undefined
})

type TimeslotFormState = TimeslotRequest & {
  duration: number
}

const timeslotForm = useEntityForm<TimeslotRequest, TimeslotFormState, TimeslotResponse>({
  validationRules: (formState) => {
    const timeslotRules = timeslotRequestRules(formState)
    return {
      ...timeslotRules,
      duration: combineRules(required(), withinRange(60, 180, '1 - 3h allowed')),
      performerId:
        performers.linkablePerformers.length > 0 ? timeslotRules.performerId : alwaysValid()
    }
  },
  entity: props.timeslot,
  formStateInitializeFn: (entity) => {
    return ref({
      scheduleId: props.scheduleId,
      startsAt: entity?.startsAt ?? props.start.toISOString(),
      duration: entity
        ? Math.round((parseTime(entity.endsAt) - parseTime(entity.startsAt)) / 60000)
        : 60,
      endsAt: entity?.endsAt ?? addHours(props.start, 1).toISOString(),
      performerId: entity?.performer.id ?? defaultStartPerformerId.value ?? '',
      performanceType: entity?.performanceType ?? PerformanceType.Live,
      subtitle: entity?.subtitle ?? '',
      file: null,
      replaceMedia: false
    })
  },
  createPersistentEntityFn: schedules.createTimeslot,
  updatePersistentEntityFn: schedules.updateTimeslot,
  useProgress: true
})
const uploadProgressText = computed(() => {
  if (timeslotForm.progress.value >= 0.99) return 'Processing...'
  return `${Math.floor(timeslotForm.progress.value * 100)}%`
})

watch(
  () => timeslotForm.state.value.replaceMedia,
  (newValue: boolean | null, oldValue: boolean | null) => {
    if (oldValue === true && newValue === false) {
      timeslotForm.state.value.file = null
    }
  }
)

const onFileSelect = (e: FileUploadSelectEvent) => {
  timeslotForm.state.value.file = e.files.length > 0 ? (e.files[0] as File) : null
  timeslotForm.val.validateIfDirty('file')
}

const onFileRemove = (e: FileUploadRemoveEvent) => {
  if (e.files.length === 0) timeslotForm.state.value.file = null
}

const performerForm = useEntityForm<PerformerRequest, PerformerRequest, PerformerResponse>({
  validationRules: performerRequestRules,
  entity: undefined,
  formStateInitializeFn: () => {
    return ref({
      name: '',
      url: ''
    })
  },
  createPersistentEntityFn: performers.create
})

const isSubmitting = ref(false)
const submit = async (): Promise<Result> => {
  isSubmitting.value = true
  if (performers.linkablePerformers.length === 0) {
    const performerResult = await submitPerformerForm()
    if (!performerResult.isSuccess) {
      isSubmitting.value = false
      return err()
    }
  }
  const timeslotResult = await timeslotForm.submit()
  isSubmitting.value = false
  if (!timeslotResult.isSuccess) return err()
  emit('submitted')
  return ok()
}
const submitPerformerForm = async (): Promise<Result> => {
  const performerResult = await performerForm.submit()
  if (!performerResult.isSuccess) return err()
  const newPerformer = performers.linkablePerformers[0]
  if (!newPerformer) return err()
  timeslotForm.state.value.performerId = newPerformer.id
  return ok()
}

const reset = () => {
  timeslotForm.reset()
  performerForm.reset()
}

const durationOptions = computed((): { label: string; value: number }[] => {
  const options: { label: string; value: number }[] = []
  const schedule = schedules.schedules.find((schedule) => schedule.id === props.scheduleId)
  if (!schedule) return []

  const endOfSchedule = parseTime(schedule.endsAt)

  const timeslots = schedule.timeslots
  const timeslotsFollowingCurrent = timeslots
    .filter(
      (timeslot) => parseTime(timeslot.startsAt) > parseTime(timeslotForm.state.value.startsAt)
    )
    .sort((a, b) => parseTime(a.startsAt) - parseTime(b.startsAt))
  const nextBoundaryTime =
    timeslotsFollowingCurrent.length > 0
      ? parseTime(timeslotsFollowingCurrent[0]!.startsAt)
      : endOfSchedule

  const maxDurationMinutes = Math.floor(
    (nextBoundaryTime - parseTime(timeslotForm.state.value.startsAt)) / 60000
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
  () => timeslotForm.state.value.duration,
  (newDuration) => {
    const startDate = parseDate(timeslotForm.state.value.startsAt)
    const endDate = new Date(startDate.getTime() + newDuration * 60000)
    timeslotForm.state.value.endsAt = endDate.toISOString()
  }
)

const emit = defineEmits<{
  submitted: []
}>()

defineExpose({
  isSubmitting,
  submit,
  reset
})

onMounted(() => {
  reset()
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.timeslot-form__file-upload {
  div.form-field__input {
    gap: variables.$space-l;
  }

  &__help-text {
    display: flex;
    flex-direction: column;

    ul {
      margin: variables.$space-s;
    }
  }
}
</style>
