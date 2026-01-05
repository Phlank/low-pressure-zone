<template>
  <div class="performers-grid">
    <DataTable
      v-if="!isMobile"
      :rows="10"
      :value="performers"
      data-key="id"
      paginator>
      <template #empty>No items to display.</template>
      <template #paginatorstart>
        <Button
          label="Create Performer"
          @click="emit('create')" />
      </template>
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
            @delete="emit('delete', data)"
            @edit="emit('edit', data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-else
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="performers"
      data-key="id"
      paginator>
      <template #empty>
        <ListItem>
          <template #left>No items to display.</template>
        </ListItem>
      </template>
      <template #list="{ items }: { items: PerformerResponse[] }">
        <div
          v-for="(performer, index) in items"
          :key="performer.id">
          <ListItem>
            <template #left>
              <span class="ellipsis">{{ performer.name }}</span>
              <span class="text-s ellipsis">
                {{ performer.url }}
              </span>
            </template>
            <template #right>
              <GridActions
                :show-delete="performer.isDeletable"
                :show-edit="performer.isEditable"
                @delete="emit('delete', performer)"
                @edit="emit('edit', performer)" />
            </template>
          </ListItem>
          <Divider v-if="index !== performers.length - 1" />
        </div>
      </template>
      <template #footer>
        <Button
          label="Create Performer"
          style="width: 100%"
          @click="emit('create')" />
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import { Button, Column, DataTable, DataView, Divider } from 'primevue'
import { inject, type Ref } from 'vue'
import { type PerformerResponse } from '@/api/resources/performersApi.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

defineProps<{
  performers: PerformerResponse[]
}>()

const emit = defineEmits<{
  create: []
  edit: [performer: PerformerResponse]
  delete: [performer: PerformerResponse]
}>()
</script>
