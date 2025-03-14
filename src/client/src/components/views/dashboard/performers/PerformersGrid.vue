<template>
  <div class="performers-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="performers"
        data-key="id">
        <Column
          field="name"
          header="Name" />
        <Column
          field="url"
          header="URL" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: PerformerResponse }">
            <GridActions
              :show-edit="data.isEditable"
              :show-delete="data.isDeletable"
              @edit="handleEditActionClick(data)"
              @delete="handleDeleteActionClick(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <div
        v-for="(performer, index) in performers"
        :key="performer.id">
        <ListItem style="width: 100%">
          <template #left>
            <div style="display: flex; flex-direction: column; overflow-x: hidden">
              <span>{{ performer.name }}</span>
              <span class="text-s ellipsis">
                {{ performer.url }}
              </span>
            </div>
          </template>
          <template #right>
            <GridActions
              :show-edit="performer.isEditable"
              :show-delete="performer.isDeletable"
              @edit="handleEditActionClick(performer)"
              @delete="handleDeleteActionClick(performer)" />
          </template>
        </ListItem>
        <Divider v-if="index != performers.length - 1" />
      </div>
    </div>
    <FormDialog
      :visible="showEditDialog"
      header="Edit Performer"
      :is-submitting="isSubmitting"
      @close="showEditDialog = false"
      @save="handleSave">
      <PerformerForm
        ref="editForm"
        :initial-state="editFormInitialState"
        :disabled="isSubmitting" />
    </FormDialog>
    <DeleteDialog
      entity-type="performer"
      :entity-name="deletingName"
      header="Delete Performer"
      :is-submitting="isSubmitting"
      :visible="showDeleteDialog"
      @close="showDeleteDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showDeleteSuccessToast, showEditSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, Divider, useToast } from 'primevue'
import { inject, ref, useTemplateRef, type Ref } from 'vue'
import performersApi, { type PerformerRequest, type PerformerResponse } from '@/api/resources/performersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const toast = useToast()
const props = defineProps<{
  performers: PerformerResponse[]
}>()

const isSubmitting = ref(false)

// EDITING PERFORMERS

const showEditDialog = ref(false)
let editingId = ''
const editFormInitialState: Ref<PerformerRequest> = ref({ name: '', url: '' })
const editForm = useTemplateRef('editForm')

const handleEditActionClick = (performer: PerformerResponse) => {
  editingId = performer.id
  editFormInitialState.value.name = performer.name
  editFormInitialState.value.url = performer.url
  showEditDialog.value = true
}

const handleSave = async () => {
  if (editForm.value == undefined) return
  const isValid = editForm.value.validation.validate()
  if (!isValid) return

  if (
    editForm.value.formState.name === editFormInitialState.value.name &&
    editForm.value.formState.url === editFormInitialState.value.url
  ) {
    showEditDialog.value = false
    return
  }

  isSubmitting.value = true
  const response = await performersApi.put(editingId, editForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, editForm.value.validation)) return

  showEditSuccessToast(toast, 'performer', editFormInitialState.value.name)
  const performerInGrid = props.performers.find((performer) => performer.id == editingId)
  if (performerInGrid) {
    performerInGrid.name = editForm.value.formState.name
    performerInGrid.url = editForm.value.formState.url
  }
  showEditDialog.value = false
}

// DELETING PERFORMERS

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')

const handleDeleteActionClick = (performer: PerformerResponse) => {
  deletingId = performer.id
  deletingName.value = performer.name
  showDeleteDialog.value = true
}

const handleDelete = async () => {
  isSubmitting.value = true
  const response = await performersApi.delete(deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  const performerInGrid = props.performers.find((performer) => performer.id == deletingId)

  props.performers.splice(props.performers.indexOf(performerInGrid!), 1)
  showDeleteSuccessToast(toast, 'performer', deletingName.value)
  showDeleteDialog.value = false
}
</script>
