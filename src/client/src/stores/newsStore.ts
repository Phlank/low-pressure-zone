import { defineStore } from 'pinia'
import newsApi, { type NewsRequest, type NewsResponse } from '@/api/resources/newsApi.ts'
import { computed, ref, type Ref } from 'vue'
import delay from '@/utils/delay.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import { err, ok, type Result } from '@/types/result.ts'
import { useToast } from 'primevue'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { getEntity } from '@/utils/arrayUtils.ts'

export const useNewsStore = defineStore('newsStore', () => {
  const items: Ref<NewsResponse[]> = ref([])
  const isLoading = ref(true)
  const toast = useToast()

  const autoRefreshing = ref(false)
  const autoRefresh = async () => {
    if (autoRefreshing.value) return
    autoRefreshing.value = true
    while (autoRefreshing.value) {
      await delay(300000)
      await refresh()
    }
  }

  const refresh = async () => {
    const response = await newsApi.get()
    if (tryHandleUnsuccessfulResponse(response, toast)) return
    items.value = response.data()
  }
  refresh().then(() => {
    isLoading.value = false
    autoRefresh().then(() => {})
  })

  const create = async (
    formState: Ref<NewsRequest>,
    validation: FormValidation<NewsRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await newsApi.post(formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    const item: NewsResponse = {
      id: response.getCreatedId(),
      createdAt: new Date().toUTCString(),
      ...formState.value
    }
    items.value.unshift(item)
    showSuccessToast(toast, 'Created', 'News', formState.value.title)
    return ok()
  }

  const update = async (
    id: string,
    formState: Ref<NewsRequest>,
    validation: FormValidation<NewsRequest>
  ): Promise<Result<void, void>> => {
    const entity = getEntity(items.value, id)
    if (!entity) return err()
    if (!validation.validate()) return err()
    const response = await newsApi.put(id, formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    entity.title = formState.value.title
    entity.body = formState.value.body
    showSuccessToast(toast, 'Updated', 'News', formState.value.title)
    return ok()
  }

  const remove = async (id: string): Promise<Result> => {
    const isInSet = items.value.some((item) => item.id === id)
    if (!isInSet) return err()
    const response = await newsApi.delete(id)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    items.value = items.value.filter((item) => item.id !== id)
    showSuccessToast(toast, 'Deleted', 'News', id)
    return ok()
  }

  const getItems = computed(() => items.value)

  return { items: getItems, create, update, remove }
})
