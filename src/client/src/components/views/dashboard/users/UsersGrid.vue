<template>
  <div class="users-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="users"
        data-key="id">
        <Column
          field="displayName"
          header="Name" />
        <Column header="Registration Date">
          <template #body="{ data }: { data: UserResponse }">
            <div v-if="data.registrationDate">
              {{ parseDate(data.registrationDate).toLocaleDateString() }}
            </div>
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <div
        v-for="(user, index) in users"
        :key="user.id">
        <ListItem style="width: 100%">
          <template #left>
            <div style="display: flex; flex-direction: column; overflow-x: hidden">
              <span class="ellipsis">{{ user.displayName }}</span>
              <span
                v-if="user.registrationDate"
                class="text-s ellipsis">
                {{ parseDate(user.registrationDate).toLocaleDateString() }}
              </span>
            </div>
          </template>
        </ListItem>
        <Divider v-if="index !== users.length - 1" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { Column, DataTable, Divider } from 'primevue'
import { inject, type Ref } from 'vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

defineProps<{
  users: UserResponse[]
}>()
</script>
