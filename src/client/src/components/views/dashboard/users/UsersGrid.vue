<template>
  <div class="users-grid">
    <DataTable
      :value="users"
      data-key="id">
      <Column
        field="username"
        header="Username" />
      <Column
        v-if="!isMobile"
        field="email"
        header="Email" />
      <Column
        v-if="!isMobile"
        header="Registration Date">
        <template #body="{ data }: { data: UserResponse }">
          <div v-if="data.registrationDate">
            {{ parseDate(data.registrationDate).toLocaleDateString() }}
          </div>
        </template>
      </Column>
      <Column
        v-if="isMobile"
        class="grid-action-col grid-action-col--1">
        <template #body>
          <GridActions
            :show-info="true"
            @info="" />
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import type { UserResponse } from '@/api/users/userResponse'
import GridActions from '@/components/data/GridActions.vue'
import { parseDate } from '@/utils/dateUtils'
import { DataTable, Column } from 'primevue'
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = defineProps<{
  users: UserResponse[]
}>()
</script>
