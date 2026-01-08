<template>
  <FormArea class="soundclash-form">
    <IftaFormField label="Start Time" input-id="startInput" size="m">
      <DatePicker v-model=""
    </IftaFormField>
  </FormArea>
</template>

<script setup lang="ts">
import FormArea from "@/components/form/FormArea.vue";
import IftaFormField from "@/components/form/IftaFormField.vue";
import {useEntityForm} from "@/composables/useEntityForm.ts";
import type {SoundclashRequest, SoundclashResponse} from "@/api/resources/soundclashApi.ts";
import {ref} from "vue";

const props = defineProps<{
  soundclash?: SoundclashResponse
  scheduleId: string
}>()

type SoundclashFormState = SoundclashRequest & {
  startTime: Date
  endTime: Date
}

const formStateInitializeFn = (entity?: SoundclashResponse | undefined) => {
  return ref<SoundclashFormState>({
    scheduleId: props.scheduleId,
    startsAt: entity?.startsAt ?? 
    title: entity?.title || '',
    description: entity?.description || '',
    startTime: entity ? new Date(entity.startTime) : new Date(),
    endTime: entity ? new Date(entity.endTime) : new Date(),
    volumeLevel: entity?.volumeLevel || 5,
  })
}

const { state, val, isSubmitting, submit, reset } = useEntityForm<({
  entity: props.soundclash,
  formStateInitializeFn: (entity) => ref{

  }
})
</script>

<style scoped lang="scss">

</style>
