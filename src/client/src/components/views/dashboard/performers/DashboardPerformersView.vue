<template>
  <div class="performers-dashboard">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="mine">Mine</Tab>
        <Tab value="all"> All</Tab>
      </TabList>
      <TabPanels v-if="!performers.isLoading">
        <TabPanel value="mine">
          <PerformersGrid
            :performers="performers.linkablePerformers"
            @create="handleCreate"
            @delete="handleDeleteAction"
            @edit="handleEdit" />
        </TabPanel>
        <TabPanel value="all">
          <PerformersGrid
            :performers="performers.items"
            @create="handleCreate"
            @delete="handleDeleteAction"
            @edit="handleEdit" />
        </TabPanel>
      </TabPanels>
    </Tabs>
    <FormDrawer
      v-model:visible="showPerformerForm"
      :is-submitting="performerFormRef?.isSubmitting"
      :title="editingPerformer ? `Edit Performer - ${editingPerformer.name}` : 'Create Performer'"
      @reset="performerFormRef?.reset()"
      @submit="performerFormRef?.submit()">
      <PerformerForm
        ref="performerFormRef"
        :performer="editingPerformer"
        hide-actions
        @submitted="showPerformerForm = false" />
    </FormDrawer>
    <DeleteDialog
      v-model:visible="showDeleteDialog"
      :entity-name="deletingPerformer?.name"
      :is-submitting="isDeleteSubmitting"
      entity-type="Performer"
      header="Delete Performer"
      @delete="handleDeleteConfirm" />
    <Skeleton
      v-if="performers.isLoading"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref, type Ref, useTemplateRef } from 'vue'
import PerformersGrid from './PerformersGrid.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'

const performers = usePerformerStore()
const tabValue: Ref<string | number> = ref('mine')
const performerFormRef = useTemplateRef('performerFormRef')

const showPerformerForm = ref(false)
const editingPerformer: Ref<PerformerResponse | undefined> = ref(undefined)
const handleCreate = () => {
  editingPerformer.value = undefined
  showPerformerForm.value = true
}
const handleEdit = (performer: PerformerResponse) => {
  editingPerformer.value = performer
  showPerformerForm.value = true
}

const showDeleteDialog = ref(false)
const deletingPerformer: Ref<PerformerResponse | undefined> = ref(undefined)
const isDeleteSubmitting = ref(false)
const handleDeleteAction = (performer: PerformerResponse) => {
  deletingPerformer.value = performer
  showDeleteDialog.value = true
}
const handleDeleteConfirm = async () => {
  if (!deletingPerformer.value) return
  isDeleteSubmitting.value = true
  const result = await performers.remove(deletingPerformer.value.id)
  isDeleteSubmitting.value = false
  if (!result.isSuccess) return
  showDeleteDialog.value = false
  deletingPerformer.value = undefined
}
</script>
