<template>
  <div class="news-grid">
    <DataTable
      v-if="!isMobile"
      :rows="10"
      :value="news.users"
      data-key="id"
      paginator>
      <template #empty>No data to display.</template>
      <Column
        field="title"
        header="Title" />
      <Column
        field="date"
        header="Date">
        <template #body="{ data }: { data: NewsResponse }">
          {{ parseDate(data.createdAt).toLocaleDateString() }}
        </template>
      </Column>
      <Column class="grid-action-col grid-action-col--2">
        <template #body="{ data }: { data: NewsResponse }">
          <GridActions
            show-delete
            show-edit
            @delete="emit('delete', data)"
            @edit="emit('edit', data)" />
        </template>
      </Column>
      <template #footer>
        <Button
          label="Create News Item"
          @click="emit('create')" />
      </template>
    </DataTable>
    <DataView
      v-else
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="news.users"
      paginator>
      <template #empty>
        <ListItem>
          <template #left>No data to display.</template>
        </ListItem>
      </template>
      <template #list="{ items }: { items: NewsResponse[]; index: number }">
        <div
          v-for="(newsItem, index) in items"
          :key="newsItem.id">
          <ListItem>
            <template #left>
              <span>{{ newsItem.title }}</span>
              <span>{{ parseDate(newsItem.createdAt).toLocaleDateString() }}</span>
            </template>
            <template #right>
              <GridActions
                show-delete
                show-edit
                @delete="emit('delete', newsItem)"
                @edit="emit('edit', newsItem)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
      <template #footer>
        <Button
          label="Create News Item"
          style="width: 100%"
          @click="emit('create')" />
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import { Button, Column, DataTable, DataView, Divider } from 'primevue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import type { NewsResponse } from '@/api/resources/newsApi.ts'
import { parseDate } from '@/utils/dateUtils.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import { inject, type Ref } from 'vue'
import { useNewsStore } from '@/stores/newsStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const news = useNewsStore()

const emit = defineEmits<{
  create: []
  edit: [newsItem: NewsResponse]
  delete: [newsItem: NewsResponse]
}>()
</script>
