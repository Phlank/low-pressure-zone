<template>
  <div class="performers-dashboard">
    <Menubar
      :model="menubarItems"
      style="width: fit-content" />
    <LinkedPerformers
      v-show="isLoaded && selectedItem === 'Mine'"
      :performers="performers" />
    <AllPerformers
      v-show="isLoaded && selectedItem === 'All'"
      :performers="performers" />
    <Skeleton
      v-show="!isLoaded"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import { Menubar, Skeleton } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { computed, onMounted, ref, type Ref } from 'vue'
import AllPerformers from './AllPerformers.vue'
import LinkedPerformers from './LinkedPerformers.vue'

const isLoaded = ref(false)
const isSubmitting = ref(false)
const isDialogOpen = ref(false)
const controlsDisabled = computed(() => !isLoaded.value || isSubmitting.value || isDialogOpen.value)

const selectedItem = ref('Mine')
const menubarItems: Ref<MenuItem[]> = ref([
  {
    label: 'Mine',
    key: 'Mine',
    command: () => (selectedItem.value = 'Mine')
  },
  {
    label: 'All',
    key: 'All',
    command: () => (selectedItem.value = 'All')
  }
])

const performers: Ref<PerformerResponse[]> = ref([])

onMounted(async () => {
  await loadPerformers()
})

const loadPerformers = async () => {
  const response = await api.performers.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
}
</script>
