<template>
  <div class="community-relationships-grid">
    <DataTable
      :rows="10"
      :value="relationships"
      paginator>
      <template #paginatorstart>
        <Button
          :disabled="usernames.length === 0"
          label="Add User"
          @click="handleAddUserClick" />
      </template>
      <Column
        field="username"
        header="Username" />
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
            show-edit
            @edit="handleEditActionClicked(data)" />
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable } from 'primevue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'
import { ref, type Ref, watch } from 'vue'
import type { UsernameResponse } from '@/api/resources/usersApi.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'

const selectedUsername: Ref<UsernameResponse | undefined> = ref(undefined)

const props = defineProps<{
  relationships: CommunityRelationshipResponse[]
  usernames: UsernameResponse[]
}>()

const handleAddUserClick = async () => {}

const handleEditActionClicked = async (data: CommunityRelationshipResponse) => {}

watch(
  props.usernames,
  (newValue) => {
    if (newValue.length === 0) selectedUsername.value = undefined
    else selectedUsername.value = newValue[0]
  },
  { immediate: true }
)
</script>
