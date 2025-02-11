<template>
  <div class="desktop-inline">
    <ScheduleForm ref="createForm" :audiences="audiences" :disabled="false" />
    <Button class="input" label="Create" @click="handleCreateClick" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { onMounted, ref, useTemplateRef, type Ref } from 'vue'

const toast = useToast()
const isSubmitting = ref(false)
const isLoading = ref(false)

onMounted(async () => {
  isLoading.value = true
  await Promise.all([loadSchedules(), loadAudiences()])
  isLoading.value = false
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])
const loadSchedules = async () => (schedules.value = (await api.schedules.get()).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])

const createForm = useTemplateRef('createForm')
const handleCreateClick = async () => {
  if (createForm.value == undefined) return
  const isInvalid = createForm.value.validation.validate()
  if (!isInvalid) return

  isSubmitting.value = true
  const response = await api.schedules.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return
  showCreateSuccessToast(toast, 'schedule')
  await loadSchedules()
  createForm.value.reset()
}
</script>
