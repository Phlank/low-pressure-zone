<template>
  <div class="users-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="users"
        data-key="id">
        <Column
          field="username"
          header="Username" />
        <Column
          field="email"
          header="Email" />
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
              <span class="ellipsis">{{ user.username }}</span>
              <span class="text-s ellipsis">
                {{ user.email }}
              </span>
            </div>
          </template>
        </ListItem>
        <Divider v-if="index != users.length - 1" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import type { UserResponse } from '@/api/users/userResponse'
import GridActions from '@/components/data/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { DataTable, Column, Divider } from 'primevue'
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = defineProps<{
  users: UserResponse[]
}>()
</script>
