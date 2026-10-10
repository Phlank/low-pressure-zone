<template>
  <div class="news-list-tab">
    <NewsGrid
      @create="handleCreate"
      @delete="handleDeleteAction"
      @edit="handleEdit" />
    <FormDrawer
      v-model:visible="isDrawerVisible"
      :is-submitting="newsFormRef?.isSubmitting"
      :title="editingNewsItem ? `Edit News - ${editingNewsItem.title}` : 'Create News'"
      @reset="newsFormRef?.reset"
      @submit="newsFormRef?.submit">
      <NewsForm
        ref="newsFormRef"
        :news-item="editingNewsItem"
        hide-submit
        @submitted="isDrawerVisible = false" />
    </FormDrawer>
    <DeleteDialog
      @hide="isDeleteDialogVisible = false"
      v-model:visible="isDeleteDialogVisible"
      :is-submitting="isDeleteSubmitting"
      entity-type="News"
      header="Delete News"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import { ref, type Ref, useTemplateRef } from 'vue'
import type { NewsResponse } from '@/api/resources/newsApi.ts'
import FormDrawer from '@/components/form/FormDrawer.vue'
import NewsForm from '@/components/form/requestForms/NewsForm.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import NewsGrid from '@/components/views/admin/news/NewsGrid.vue'
import { useNewsStore } from '@/stores/newsStore.ts'

const news = useNewsStore()
const newsFormRef = useTemplateRef('newsFormRef')
const isDrawerVisible = ref(false)

const handleCreate = () => {
  editingNewsItem.value = undefined
  isDrawerVisible.value = true
}

const editingNewsItem: Ref<NewsResponse | undefined> = ref(undefined)
const handleEdit = (newsItem: NewsResponse) => {
  editingNewsItem.value = newsItem
  isDrawerVisible.value = true
}

const deletingId = ref('')
const isDeleteDialogVisible = ref(false)
const isDeleteSubmitting = ref(false)
const handleDeleteAction = (newsItem: NewsResponse) => {
  deletingId.value = newsItem.id
  isDeleteDialogVisible.value = true
}
const handleDelete = async () => {
  isDeleteSubmitting.value = true
  const result = await news.remove(deletingId.value)
  isDeleteSubmitting.value = false
  if (!result.isSuccess) return
  isDeleteDialogVisible.value = false
  deletingId.value = ''
}
</script>
