import { defineStore } from 'pinia'
import newsApi, { type NewsResponse } from '@/api/resources/newsApi.ts'
import { computed, ref, type Ref } from 'vue'
import { useToast } from 'primevue'
import { showDeleteSuccessToast, showSuccessToast } from '@/utils/toastUtils.ts'
import { removeEntity } from '@/utils/arrayUtils.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFns.ts'

export const useNewsStore = defineStore('newsStore', () => {
  const users: Ref<NewsResponse[]> = ref([])
  const toast = useToast()

  useRefresh(newsApi.get, (data) => {
    users.value = data
  })

  const create = useCreatePersistentItemFn(
    newsApi.post,
    (id, form) => {
      const entity: NewsResponse = {
        id: id,
        createdAt: new Date().toUTCString(),
        ...form
      }
      users.value.unshift(entity)
      showSuccessToast(toast, 'Created', 'News', form.title)
    },
    toast
  )

  const update = useUpdatePersistentItemFn(
    users,
    newsApi.put,
    (form, entity) => {
      entity.title = form.title
      entity.body = form.body
      showSuccessToast(toast, 'Updated', 'News', form.title)
    },
    toast
  )

  const remove = useRemovePersistentItemFn(
    users,
    newsApi.delete,
    (entity) => {
      removeEntity(users.value, entity.id)
      showDeleteSuccessToast(toast, 'News', entity.title)
    },
    toast
  )
  const getItems = computed(() => users.value)

  return { users: getItems, create, update, remove }
})
