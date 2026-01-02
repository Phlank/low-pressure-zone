<template>
  <div class="community-relationships-grid">
    <DataTable
      v-if="!isMobile"
      :rows="10"
      :value="communities.getRelationships(props.community.id)"
      paginator>
      <template #paginatorstart>
        <Button
          v-if="availableUsers.length !== 0"
          label="Add User"
          @click="emit('create')" />
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
            @edit="emit('edit', data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-if="isMobile"
      :paginator="communities.getRelationships(props.community.id).length > 5"
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="communities.getRelationships(props.community.id)"
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
                @edit="emit('edit', relationship)" />
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
          @click="emit('create')" />
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, DataView, Divider } from 'primevue'
import { type CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'
import { computed, type ComputedRef, inject, type Ref } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { useUserStore } from '@/stores/userStore.ts'
import ListItem from '@/components/data/ListItem.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const users = useUserStore()
const communities = useCommunityStore()

const props = defineProps<{
  community: CommunityResponse
}>()

const availableUsers: ComputedRef<UserResponse[]> = computed(() => {
  const userIdsInUse = communities
    .getRelationships(props.community.id)
    .map((relationship) => relationship.userId)
  const usersNotInUse = users.users.filter((user) => userIdsInUse.indexOf(user.id) === -1)
  return usersNotInUse.filter((user) => !user.isAdmin)
})

const getMobileRelationshipText = (relationship: CommunityRelationshipResponse) => {
  const roles = []
  if (relationship.isPerformer) roles.push('Performer')
  if (relationship.isOrganizer) roles.push('Organizer')
  return roles.join(', ')
}

const emit = defineEmits<{
  create: []
  edit: [relationship: CommunityRelationshipResponse]
}>()
</script>
