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
  const newsItems: Ref<NewsResponse[]> = ref([])
  const toast = useToast()

  useRefresh(newsApi.get, (data) => {
    newsItems.value = data
  })

  const create = useCreatePersistentItemFn(
    newsApi.post,
    (id, form) => {
      const entity: NewsResponse = {
        id: id,
        createdAt: new Date().toUTCString(),
        ...form
      }
      newsItems.value.unshift(entity)
      showSuccessToast(toast, 'Created', 'News', form.title)
    },
    toast
  )

  const update = useUpdatePersistentItemFn(
    newsItems,
    newsApi.put,
    (form, entity) => {
      entity.title = form.title
      entity.body = form.body
      showSuccessToast(toast, 'Updated', 'News', form.title)
    },
    toast
  )

  const remove = useRemovePersistentItemFn(
    newsItems,
    newsApi.delete,
    (entity) => {
      removeEntity(newsItems.value, entity.id)
      showDeleteSuccessToast(toast, 'News', entity.title)
    },
    toast
  )
  const getNewsItems = computed(() => newsItems.value)

  return { newsItems: getNewsItems, create, update, remove }
})
