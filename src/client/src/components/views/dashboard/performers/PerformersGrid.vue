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
              :show-delete="data.isDeletable"
              :show-edit="data.isEditable"
              @delete="handleDeleteActionClick(data)"
              @edit="handleEditActionClick(data)" />
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
              :show-delete="performer.isDeletable"
              :show-edit="performer.isEditable"
              @delete="handleDeleteActionClick(performer)"
              @edit="handleEditActionClick(performer)" />
          </template>
        </ListItem>
        <Divider v-if="index !== performers.length - 1" />
      </div>
    </div>
    <Dialog
      v-model:visible="showEditDialog"
      :draggable="false"
      header="Edit Performer"
      modal>
      <PerformerForm
        :initial-state="editFormInitialState"
        :performer-id="editingId"
        align-actions="right"
        @after-submit="showEditDialog = false" />
    </Dialog>
    <DeleteDialog
      :entity-name="deletingName"
      :is-submitting="isSubmitting"
      :visible="showDeleteDialog"
      entity-type="performer"
      header="Delete Performer"
      @close="showDeleteDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showDeleteSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, Dialog, Divider, useToast } from 'primevue'
import { inject, ref, type Ref } from 'vue'
import performersApi, {
  type PerformerRequest,
  type PerformerResponse
} from '@/api/resources/performersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const toast = useToast()
const performerStore = usePerformerStore()

defineProps<{
  performers: PerformerResponse[]
}>()

const showEditDialog = ref(false)
const editingId = ref('')
const editFormInitialState: Ref<PerformerRequest> = ref({ name: '', url: '' })

const handleEditActionClick = async (performer: PerformerResponse) => {
  editingId.value = performer.id
  editFormInitialState.value.name = performer.name
  editFormInitialState.value.url = performer.url
  showEditDialog.value = true
}

const isSubmitting = ref(false)
const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')

const handleDeleteActionClick = async (performer: PerformerResponse) => {
  deletingId = performer.id
  deletingName.value = performer.name
  showDeleteDialog.value = true
}

const handleDelete = async () => {
  isSubmitting.value = true
  const response = await performersApi.delete(deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  showDeleteSuccessToast(toast, 'performer', deletingName.value)
  showDeleteDialog.value = false
  performerStore.remove(deletingId)
}
</script>
