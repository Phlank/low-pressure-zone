import { defineStore } from 'pinia'
import type { PerformerRequest, PerformerResponse } from '@/api/resources/performersApi.ts'
import performersApi from '@/api/resources/performersApi.ts'
import { computed, type Ref, ref } from 'vue'
import { useToast } from 'primevue'
import { addAlphabetically, getEntityMap, removeEntity } from '@/utils/arrayUtils.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFns.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils.ts'
import { useAuthStore } from '@/stores/authStore.ts'

export const usePerformerStore = defineStore('performerStore', () => {
  const performers: Ref<PerformerResponse[]> = ref([])
  const performersMap = ref(getEntityMap(performers.value))
  const toast = useToast()
  const auth = useAuthStore()

  const { isLoading } = useRefresh(
    performersApi.get,
    (data) => {
      performers.value = data
      performersMap.value = getEntityMap(performers.value)
    },
    { permissionFn: () => auth.isLoggedIn }
  )

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
      showCreateSuccessToast(toast, 'Performer', form.name)
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
      showEditSuccessToast(toast, 'Performer', form.name)
    },
    toast
  )

  const remove = useRemovePersistentItemFn(
    performers,
    performersApi.delete,
    (entity) => {
      performersMap.value[entity.id] = undefined
      removeEntity(performers.value, entity.id)
      showDeleteSuccessToast(toast, 'Performer', entity.name)
    },
    toast
  )

  const getPerformers = computed(() => performers.value)
  const getLinkablePerformers = computed(() =>
    performers.value.filter((performer) => performer.isLinkableToTimeslot)
  )

  return {
    isLoading: isLoading,
    performers: getPerformers,
    linkablePerformers: getLinkablePerformers,
    getById,
    create,
    update,
    remove
  }
})
