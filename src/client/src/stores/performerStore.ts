import { defineStore } from 'pinia'
import type { PerformerRequest, PerformerResponse } from '@/api/resources/performersApi.ts'
import performersApi from '@/api/resources/performersApi.ts'
import { computed, type Ref, ref } from 'vue'
import delay from '@/utils/delay.ts'
import { useToast } from 'primevue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { addAlphabetically, getEntityMap, removeEntity } from '@/utils/arrayUtils.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFunctions.ts'

export const usePerformerStore = defineStore('performerStore', () => {
  const performers: Ref<PerformerResponse[]> = ref([])
  const performersMap = ref(getEntityMap(performers.value))
  const toast = useToast()

  let autoRefreshing = false
  const isLoading = ref(true)
  const autoRefresh = async () => {
    if (autoRefreshing) return
    autoRefreshing = true
    while (autoRefreshing) {
      await delay(300000)
      await refresh()
    }
  }
  const refresh = async () => {
    const response = await performersApi.get()
    if (tryHandleUnsuccessfulResponse(response, toast)) return
    performers.value = response.data()
    performersMap.value = getEntityMap(performers.value)
  }
  refresh().then(() => {
    isLoading.value = false
    autoRefresh().then(() => {})
  })

  const getById = (id: string) => performersMap.value[id]

  const create = useCreatePersistentItemFn(
    performersApi.post,
    (id, form) => {
      const performer: PerformerResponse = {
        id: id,
        ...form,
        isDeletable: true,
        isEditable: true,
        isLinkableToTimeslot: true
      }
      addAlphabetically(performers.value, performer, (item) => item.name)
      performersMap.value[performer.id] = performer
    },
    toast
  )

  const update = useUpdatePersistentItemFn<PerformerRequest, PerformerResponse>(
    performers,
    performersApi.put,
    (form, entity) => {
      entity.url = form.url
      if (entity.name !== form.name) {
        entity.name = form.name
        performers.value.sort((a, b) => a.name.localeCompare(b.name))
      }
    },
    toast
  )

  const remove = useRemovePersistentItemFn(
    performers,
    performersApi.delete,
    (entity) => {
      performersMap.value[entity.id] = undefined
      removeEntity(performers.value, entity.id)
    },
    toast
  )

  const getIsLoading = computed(() => isLoading.value)
  const getPerformers = computed(() => performers.value)
  const getLinkablePerformers = computed(() =>
    performers.value.filter((performer) => performer.isLinkableToTimeslot)
  )

  return {
    isLoading: getIsLoading,
    items: getPerformers,
    linkablePerformers: getLinkablePerformers,
    getById,
    create,
    update,
    remove
  }
})
