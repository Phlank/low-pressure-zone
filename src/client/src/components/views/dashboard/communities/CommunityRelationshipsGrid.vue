<template>
  <div class="community-relationships-grid">
    <div class="community-relationships-grid__data">
      <DataTable
        v-if="!isMobile"
        :rows="10"
        :value="relationships"
        paginator>
        <template #paginatorstart>
          <Button
            v-if="availableUsers.length !== 0"
            label="Add User"
            @click="handleAddUserClick" />
        </template>
        <template #empty>No items to display.</template>
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
          <template #body="{ data }: { data: CommunityRelationshipResponse }">
            <GridActions
              :show-edit="data.isEditable"
              @edit="handleEditActionClick(data)" />
          </template>
        </Column>
      </DataTable>
      <DataView
        v-if="isMobile"
        :paginator="relationships.length > 5"
        :rows="5"
        :value="relationships"
        data-key="id">
        <template #empty>
          <ListItem>
            <template #left>No items to display.</template>
          </ListItem>
        </template>
        <template #list="{ items }: { items: CommunityRelationshipResponse[] }">
          <div
            v-for="(relationship, index) in items"
            :key="relationship.userId">
            <ListItem>
              <template #left>
                <span>{{ relationship.displayName }}</span>
                <span class="text-s">{{ getMobileRelationshipText(relationship) }}</span>
              </template>
              <template #right>
                <GridActions
                  :show-edit="relationship.isEditable"
                  @edit="handleEditActionClick(relationship)" />
              </template>
            </ListItem>
            <Divider v-if="index < items.length - 1" />
          </div>
        </template>
        <template #footer>
          <Button
            v-if="availableUsers.length !== 0"
            label="Add User"
            style="width: 100%"
            @click="handleAddUserClick" />
        </template>
      </DataView>
      <FormArea>
        <template #actions></template>
      </FormArea>
    </div>
    <FormDialog
      :is-submitting="isSubmitting"
      :visible="showCreateDialog"
      header="Create Relationship"
      @close="showCreateDialog = false"
      @save="handleCreateDialogSave()">
      <CommunityRelationshipForm
        ref="createForm"
        :available-users="availableUsers" />
    </FormDialog>
    <FormDialog
      :is-submitting="isSubmitting"
      :visible="showUpdateDialog"
      header="Update Relationship"
      @close="showUpdateDialog = false"
      @save="handleEditDialogSave()">
      <CommunityRelationshipForm
        ref="updateForm"
        :available-users="[userStore.getUser(updatingUserId)]"
        :initial-state="updatingRelationship" />
    </FormDialog>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, DataView, Divider, useToast } from 'primevue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import { computed, type ComputedRef, inject, onMounted, ref, type Ref, useTemplateRef } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import CommunityRelationshipForm from '@/components/form/requestForms/CommunityRelationshipForm.vue'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { showCreateSuccessToast, showEditSuccessToast } from '@/utils/toastUtils.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { useUserStore } from '@/stores/userStore.ts'
import ListItem from '@/components/data/ListItem.vue'
import FormArea from '@/components/form/FormArea.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const toast = useToast()
const userStore = useUserStore()

const props = defineProps<{
  community: CommunityResponse
  relationships: CommunityRelationshipResponse[]
}>()

const emit = defineEmits<{
  update: []
}>()

onMounted(async () => {
  if (userStore.users.length === 0) await userStore.loadUsersAsync()
})

const availableUsers: ComputedRef<UserResponse[]> = computed(() => {
  const userIdsInUse = props.relationships.map((relationship) => relationship.userId)
  return userStore.users.filter((user) => userIdsInUse.indexOf(user.id) === -1)
})

const getMobileRelationshipText = (relationship: CommunityRelationshipResponse) => {
  const roles = []
  if (relationship.isPerformer) roles.push('Performer')
  if (relationship.isOrganizer) roles.push('Organizer')
  return roles.join(', ')
}

const createForm = useTemplateRef('createForm')
const isSubmitting: Ref<boolean> = ref(false)
const showCreateDialog: Ref<boolean> = ref(false)
const handleAddUserClick = async () => {
  updatingRelationship.value = undefined
  showCreateDialog.value = true
}
const handleCreateDialogSave = async () => {
  if (!createForm.value) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communityRelationshipsApi.put(
    props.community.id,
    createForm.value.formState.userId,
    createForm.value.formState
  )
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  const displayName = userStore.getUser(createForm.value.formState.userId).displayName
  if (updatingRelationship.value) showEditSuccessToast(toast, 'relationship', displayName)
  else showCreateSuccessToast(toast, 'relationship', displayName)
  showCreateDialog.value = false
  emit('update')
}

const updateForm = useTemplateRef('updateForm')
const showUpdateDialog = ref(false)
const updatingRelationship: Ref<CommunityRelationshipResponse | undefined> = ref(undefined)
const updatingUserId: Ref<string> = ref('')

const handleEditActionClick = (relationship: CommunityRelationshipResponse) => {
  updatingRelationship.value = relationship
  updatingUserId.value = relationship.userId
  showUpdateDialog.value = true
}

const handleEditDialogSave = async () => {
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

  const displayName = userStore.getUser(updatingUserId.value).displayName
  showEditSuccessToast(toast, 'relationship', displayName)
  showUpdateDialog.value = false
  emit('update')
}
</script>
