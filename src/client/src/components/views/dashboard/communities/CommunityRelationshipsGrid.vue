<template>
  <div class="community-relationships-grid">
    <DataTable :value="relationships">
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
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import { Column, DataTable } from 'primevue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'

defineProps<{
  relationships: CommunityRelationshipResponse[]
}>()
</script>
