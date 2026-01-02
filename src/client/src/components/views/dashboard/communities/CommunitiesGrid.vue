<template>
  <div class="communities-grid">
    <DataTable
      v-if="!isMobile"
      :rows="10"
      :value="communities.communities"
      data-key="id"
      paginator>
      <template #paginatorstart>
        <Button
          v-if="auth.isInRole(roles.admin)"
          label="Create Community"
          style="width: 100%"
          @click="emit('create')" />
      </template>
      <Column
        field="name"
        header="Name" />
      <Column header="Roles">
        <template #body="{ data }: { data: CommunityResponse }">
          <div
            v-for="role in getCommunityRoles(data)"
            :key="role">
            {{ role }}
          </div>
        </template>
      </Column>
      <Column
        field="url"
        header="URL" />
      <Column class="grid-action-col grid-action-col--2">
        <template #body="{ data }: { data: CommunityResponse }">
          <GridActions
            :show-delete="data.isDeletable"
            :show-edit="data.isEditable"
            @delete="emit('delete', data)"
            @edit="emit('edit', data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-else
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="communities.communities"
      paginator>
      <template #list="{ items }: { items: CommunityResponse[] }">
        <div
          v-for="(community, index) in items"
          :key="community.id">
          <ListItem>
            <template #left>
              <span>{{ community.name }}</span>
              <span class="text-s">{{ getCommunityRoles(community).join(', ') }}</span>
            </template>
            <template #right>
              <GridActions
                :show-delete="community.isDeletable"
                :show-edit="community.isEditable"
                @delete="emit('delete', community)"
                @edit="emit('edit', community)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
      <template #footer>
        <Button
          v-if="auth.isInRole(roles.admin)"
          label="Create Community"
          style="width: 100%"
          @click="emit('create')" />
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, DataView, Divider } from 'primevue'
import { inject, type Ref } from 'vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const auth = useAuthStore()
const communities = useCommunityStore()

const getCommunityRoles = (community: CommunityResponse): string[] => {
  const roles: string[] = []
  if (community.isPerformable) roles.push('Performer')
  if (community.isOrganizable) roles.push('Organizer')
  return roles
}

const emit = defineEmits<{
  edit: [community: CommunityResponse]
  delete: [community: CommunityResponse]
  create: []
}>()
</script>
