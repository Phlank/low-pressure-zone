import { defineStore } from 'pinia'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'
import performersApi from '@/api/resources/performersApi.ts'
import { computed, type Ref, ref } from 'vue'

export const usePerformerStore = defineStore('performerStore', () => {
  const loadedPerformers: Ref<PerformerResponse[]> = ref([])

  let loadPerformersPromise: Promise<void> | undefined
  const loadPerformers = async () => {
    const response = await performersApi.get()
    if (!response.isSuccess()) {
      console.log(JSON.stringify(response))
      return
    }
    loadedPerformers.value = response.data!
  }

  const loadPerformersAsync = async () => {
    if (!loadPerformersPromise) {
      loadPerformersPromise = loadPerformers()
    }
    await loadPerformersPromise
  }

  const performers = computed(() => loadedPerformers.value)
  const linkablePerformers = computed(() =>
    loadedPerformers.value.filter((performer) => performer.isLinkableToTimeslot)
  )

  const add = (performer: PerformerResponse) => {
    const alphabeticalIndex = loadedPerformers.value.findIndex(
      (loadedPerformer) => loadedPerformer.name.toLowerCase() > performer.name.toLowerCase()
    )
    if (alphabeticalIndex > -1) {
      loadedPerformers.value.splice(alphabeticalIndex, 0, performer)
    }
  }

  const remove = (id: string) => {
    const index = loadedPerformers.value.findIndex((performer) => performer.id === id)
    if (index > -1) {
      loadedPerformers.value.splice(index, 1)
    }
  }

  const updateAsync = async (id: string) => {
    const response = await performersApi.getById(id)
    if (!response.isSuccess()) return
    const index = loadedPerformers.value.findIndex((performer) => performer.id === id)
    loadedPerformers.value.splice(index, 1, response.data!)
  }

  return {
    performers,
    linkablePerformers,
    loadPerformersAsync,
    add,
    remove,
    updateAsync
  }
})
