<template>
  <div class="desktop-inline">
    <ScheduleForm :audiences="audiences" :disabled="false" />
    <Button class="input" label="Create" @click="handleCreateClick" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { Button } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])

onMounted(async () => {
  await Promise.all([loadSchedules(), loadAudiences()])
})

const loadSchedules = async () => (schedules.value = (await api.schedules.get()).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])

const handleCreateClick = () => {}
</script>
