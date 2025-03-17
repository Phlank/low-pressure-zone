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
      @close="showUpdateDialog = false">
      <CommunityRelationshipForm
        :available-users="availableUsers"
        :initial-state="updatingRelationship" />
    </FormDialog>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable } from 'primevue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'
import { ref, type Ref } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import CommunityRelationshipForm from '@/components/form/requestForms/CommunityRelationshipForm.vue'

const props = defineProps<{
  relationships: CommunityRelationshipResponse[]
  availableUsers: UserResponse[]
}>()

const isSubmitting: Ref<boolean> = ref(false)
const showUpdateDialog: Ref<boolean> = ref(false)
const updatingRelationship: Ref<CommunityRelationshipResponse | undefined> = ref(undefined)
const handleAddUserClick = async () => {
  console.log('Grid:' + JSON.stringify(props.availableUsers))
  updatingRelationship.value = undefined
  showUpdateDialog.value = true
}
</script>
