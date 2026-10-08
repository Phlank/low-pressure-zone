<template>
  <div class="slot-form">
    <FormArea>
      <IftaFormField
        v-if="slotTypeSelections.length > 1"
        label="Slot Type"
        size="m"
        input-id="slotTypeInput"
        message="">
        <Select
          id="slotTypeInput"
          v-model:model-value="slotType"
          :options="slotTypeSelections" />
      </IftaFormField>
    </FormArea>
    <HourlySlotForm
      v-show="slotType === 'Hourly'"
      ref="hourlySlotFormComponent"
      :schedule-id="scheduleId"
      :start="start"
      @submitted="emit('submitted')" />
    <ClashSlotForm
      v-show="slotType === 'Clash'"
      ref="clashSlotFormComponent"
      :schedule-id="scheduleId"
      :start="start"
      @submitted="emit('submitted')" />
  </div>
</template>

<script setup lang="ts">
import { Select } from 'openvue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { computed, ref, useTemplateRef, watch } from 'vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import FormArea from '@/components/form/FormArea.vue'
import type { SlotResponse } from '@/api/resources/schedulesApi.ts'
import HourlySlotForm from '@/components/form/requestForms/HourlySlotForm.vue'
import ClashSlotForm from '@/components/form/requestForms/ClashSlotForm.vue'
import { defaultPromise } from '@/utils/defaults.ts'

const schedules = useScheduleStore()

const props = defineProps<{ scheduleId: string; start: Date; existingSlot?: SlotResponse }>()
const schedule = computed(() => schedules.getScheduleById(props.scheduleId))
const hourlySlotFormComponent = useTemplateRef('hourlySlotFormComponent')
const clashSlotFormComponent = useTemplateRef('clashSlotFormComponent')

const slotTypeSelections = ref<string[]>(props.existingSlot?.type ? [props.existingSlot.type] : [])
const slotType = ref(props.existingSlot?.type ?? '')
watch(
  schedule,
  () => {
    if (slotTypeSelections.value.length > 0) return
    if (schedule.value?.isHourlyAllowed) slotTypeSelections.value.push('Hourly')
    if (schedule.value?.isClashAllowed) slotTypeSelections.value.push('Clash')
    if (slotType.value === '' && schedule.value) slotType.value = slotTypeSelections.value[0]
  },
  { immediate: true }
)

const isSubmitting = computed(
  () =>
    hourlySlotFormComponent.value?.isSubmitting ||
    clashSlotFormComponent.value?.isSubmitting ||
    false
)
const reset = () => {
  if (slotType.value === 'Hourly') {
    hourlySlotFormComponent.value?.reset()
  } else {
    clashSlotFormComponent.value?.reset()
  }
}
const submit = () => {
  if (slotType.value === 'Hourly') {
    return hourlySlotFormComponent.value?.submit() ?? defaultPromise()
  } else {
    return clashSlotFormComponent.value?.submit() ?? defaultPromise()
  }
}

const emit = defineEmits<{
  submitted: []
}>()

defineExpose({ isSubmitting, reset, submit })
</script>

<style scoped lang="scss"></style>
