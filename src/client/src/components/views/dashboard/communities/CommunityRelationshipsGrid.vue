<template>
  <div class="community-relationships-grid">
    <DataTable
      :rows="10"
      :value="relationships"
      paginator>
      <template #paginatorstart>
        <Button
          :disabled="availableUsers.length === 0"
          label="Add User"
          @click="handleAddUserClick" />
      </template>
      <Column
        field="displayName"
        header="Name" />
      <Column
        field="isPerformer"
        header="Performer">
        <template #body="{ data }">
          {{ data.isPerformer ? 'Yes' : '' }}
        </template>
      </Column>
      <Column
        field="isOrganizer"
        header="Organizer">
        <template #body="{ data }">
          {{ data.isOrganizer ? 'Yes' : '' }}
        </template>
      </Column>
      <Column class="grid-action-col grid-action-col--1">
        <!--        <template #body="{ data }">-->
        <!--          <GridActions-->
        <!--            show-edit-->
        <!--            @edit="handleEditActionClick(data)" />-->
        <!--        </template>-->
      </Column>
    </DataTable>
    <FormDialog
      :is-submitting="isSubmitting"
      :visible="showUpdateDialog"
      header="Create Relationship"
      @close="showUpdateDialog = false"
      @save="handleUpdateDialogSave()">
      <CommunityRelationshipForm
        ref="updateForm"
        :available-users="availableUsers"
        :initial-state="updatingRelationship" />
    </FormDialog>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, useToast } from 'primevue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import { ref, type Ref, useTemplateRef } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import CommunityRelationshipForm from '@/components/form/requestForms/CommunityRelationshipForm.vue'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { showCreateSuccessToast, showEditSuccessToast } from '@/utils/toastUtils.ts'

const toast = useToast()
const updateForm = useTemplateRef('updateForm')

const props = defineProps<{
  availableUsers: UserResponse[]
  community: CommunityResponse
  relationships: CommunityRelationshipResponse[]
}>()

const emit = defineEmits<{
  update: []
}>()

const isSubmitting: Ref<boolean> = ref(false)
const showUpdateDialog: Ref<boolean> = ref(false)
const updatingRelationship: Ref<CommunityRelationshipResponse | undefined> = ref(undefined)
const handleAddUserClick = async () => {
  console.log('Grid:' + JSON.stringify(props.availableUsers))
  updatingRelationship.value = undefined
  showUpdateDialog.value = true
}
const handleUpdateDialogSave = async () => {
  if (!updateForm.value) return
  const isValid = updateForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communityRelationshipsApi.put(
    props.community.id,
    updateForm.value.formState.userId,
    updateForm.value.formState
  )
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  const user = props.availableUsers.find((user) => user.id === updateForm.value!.formState.userId)
  if (updatingRelationship.value) showEditSuccessToast(toast, 'relationship', user!.displayName)
  else showCreateSuccessToast(toast, 'relationship', user!.displayName)
  showUpdateDialog.value = false
  emit('update')
}
</script>
