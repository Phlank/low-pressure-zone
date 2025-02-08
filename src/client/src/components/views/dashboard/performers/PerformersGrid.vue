<template>
  <div class="performer-grid">
    <DataTable v-if="isLoaded" :value="performers">
      <Column field="name" header="Name" />
      <Column field="url" header="URL" />
      <Column field="modifiedDate" header="Last Modified">
        <template #body="slotProps">
          {{ new Date(slotProps.data.modifiedDate).toLocaleString() }}
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import { Column, DataTable } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'

const isLoaded = ref(false)
const performers: Ref<PerformerResponse[]> = ref([])

onMounted(async () => {
  const response = await api.performers.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
})
</script>
