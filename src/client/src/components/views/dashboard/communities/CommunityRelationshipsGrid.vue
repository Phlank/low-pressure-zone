<template>
  <div class="community-relationships-grid">
    <div class="community-relationships-grid__data">
      <DataTable
        v-if="!isMobile"
        :rows="10"
        :value="communityStore.getRelationships(props.community.id)"
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
        :paginator="communityStore.getRelationships(props.community.id).length > 5"
        :paginator-template="mobilePaginatorTemplate"
        :rows="5"
        :value="communityStore.getRelationships(props.community.id)"
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
import { useCommunityStore } from '@/stores/communityStore.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const toast = useToast()
const userStore = useUserStore()
const communityStore = useCommunityStore()

const props = defineProps<{
  community: CommunityResponse
}>()

onMounted(async () => {
  if (userStore.users.length === 0) await userStore.loadUsersAsync()
})

const availableUsers: ComputedRef<UserResponse[]> = computed(() => {
  const userIdsInUse = communityStore
    .getRelationships(props.community.id)
    .map((relationship) => relationship.userId)
  const usersNotInUse = userStore.users.filter((user) => userIdsInUse.indexOf(user.id) === -1)
  return usersNotInUse.filter((user) => !user.isAdmin)
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
  const formState = createForm.value.formState
  const validation = createForm.value.validation

  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communityRelationshipsApi.put(
    props.community.id,
    createForm.value.formState.userId,
    createForm.value.formState
  )
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  const displayName = userStore.getUser(formState.userId).displayName
  showCreateSuccessToast(toast, 'relationship', displayName)
  showCreateDialog.value = false
  communityStore.addRelationship(props.community.id, {
    userId: formState.userId,
    communityId: props.community.id,
    displayName: userStore.getUser(formState.userId).displayName,
    isOrganizer: formState.isOrganizer,
    isPerformer: formState.isPerformer,
    isEditable: true
  })
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
  await communityStore.updateRelationshipAsync(
    updatingRelationship.value!.communityId,
    updatingRelationship.value!.userId
  )
  updatingRelationship.value = undefined
  updatingUserId.value = ''
}
</script>
