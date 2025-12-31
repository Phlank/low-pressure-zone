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
          <template #body="{ data }: { data: CommunityRelationshipResponse }">
            {{ data.isPerformer ? 'Yes' : '' }}
          </template>
        </Column>
        <Column
          field="isOrganizer"
          header="Organizer">
          <template #body="{ data }: { data: CommunityRelationshipResponse }">
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
    <Dialog
      v-model:visible="showCreateDialog"
      :draggable="false"
      header="Create Relationship"
      modal>
      <CommunityRelationshipForm
        :available-users="availableUsers"
        :community-id="community.id"
        align-actions="right"
        @after-submit="showCreateDialog = false" />
    </Dialog>
    <Dialog
      v-model:visible="showUpdateDialog"
      :draggable="false"
      header="Update Relationship"
      modal>
      <CommunityRelationshipForm
        :available-users="[userStore.getUser(updatingUserId)!]"
        :community-id="community.id"
        :initial-state="updatingRelationship"
        align-actions="right"
        @after-submit="showUpdateDialog = false" />
    </Dialog>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, DataView, Dialog, Divider } from 'primevue'
import { type CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'
import { computed, type ComputedRef, inject, onMounted, ref, type Ref } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'
import CommunityRelationshipForm from '@/components/form/requestForms/CommunityRelationshipForm.vue'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { useUserStore } from '@/stores/userStore.ts'
import ListItem from '@/components/data/ListItem.vue'
import FormArea from '@/components/form/FormArea.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const userStore = useUserStore()
const communityStore = useCommunityStore()

const props = defineProps<{
  community: CommunityResponse
}>()

onMounted(async () => {
  if (userStore.items.length === 0) await userStore.loadUsersAsync()
})

const availableUsers: ComputedRef<UserResponse[]> = computed(() => {
  const userIdsInUse = communityStore
    .getRelationships(props.community.id)
    .map((relationship) => relationship.userId)
  const usersNotInUse = userStore.items.filter((user) => userIdsInUse.indexOf(user.id) === -1)
  return usersNotInUse.filter((user) => !user.isAdmin)
})

const getMobileRelationshipText = (relationship: CommunityRelationshipResponse) => {
  const roles = []
  if (relationship.isPerformer) roles.push('Performer')
  if (relationship.isOrganizer) roles.push('Organizer')
  return roles.join(', ')
}

const showCreateDialog: Ref<boolean> = ref(false)

const handleAddUserClick = async () => {
  updatingRelationship.value = undefined
  showCreateDialog.value = true
}

const showUpdateDialog = ref(false)
const updatingRelationship: Ref<CommunityRelationshipResponse | undefined> = ref(undefined)
const updatingUserId: Ref<string> = ref('')

const handleEditActionClick = (relationship: CommunityRelationshipResponse) => {
  updatingRelationship.value = relationship
  updatingUserId.value = relationship.userId
  showUpdateDialog.value = true
}
</script>
