<template>
  <div class="news-tab">
    <DataView
      :paginator="news.users.length > 1"
      :paginator-template="simplePaginatorTemplate"
      :rows="1"
      :value="news.users">
      <template #empty>
        <div>No news!</div>
      </template>
      <template #list="{ items }: { items: NewsResponse[] }">
        <NewsPost
          v-for="newsItem in items"
          :key="newsItem.id"
          :body="newsItem.body"
          :post-date="parseDate(newsItem.createdAt)"
          :title="newsItem.title" />
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import { DataView } from 'primevue'
import { useNewsStore } from '@/stores/newsStore.ts'
import type { NewsResponse } from '@/api/resources/newsApi.ts'
import { parseDate } from '@/utils/dateUtils.ts'
import NewsPost from '@/components/controls/NewsPost.vue'
import { simplePaginatorTemplate } from '@/constants/componentTemplates.ts'

const news = useNewsStore()
</script>
