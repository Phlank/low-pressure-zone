<template>
  <div class="users-grid">
    <div v-if="!isMobile">
      <DataTable
        :rows="10"
        :value="usersToDisplay"
        data-key="id"
        paginator>
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
        <Column header="Streamer">
          <template #body="{ data }: { data: UserResponse }">
            <span v-if="data.isStreamer">Yes</span>
            <Button
              v-else
              label="Create Streamer"
              @click="emit('createStreamer', data)" />
          </template>
        </Column>
        <template #footer>
          <Button
            v-if="authStore.isInAnyRoles(Roles.organizer, Roles.admin)"
            label="Invite New User"
            @click="emit('newInvite')" />
        </template>
      </DataTable>
    </div>
    <div v-else>
      <DataView
        :paginator-templat="mobilePaginatorTemplate"
        :paginator-template="mobilePaginatorTemplate"
        :rows="5"
        :value="usersToDisplay"
        data-key="id"
        paginator>
        <template #empty>
          <ListItem>
            <template #left>No items to display.</template>
          </ListItem>
        </template>
        <template #list="{ items }: { items: UserResponse[] }">
          <div
            v-for="(user, index) in items"
            :key="user.id">
            <ListItem>
              <template #left>
                <div style="display: flex; flex-direction: column; overflow-x: hidden">
                  <span class="ellipsis">{{ user.displayName }}</span>
                  <span class="text-s ellipsis mobile-info-text">
                    <span v-if="user.isStreamer">Streamer</span>
                    <span v-if="user.registrationDate">
                      {{ parseDate(user.registrationDate).toLocaleDateString() }}
                    </span>
                  </span>
                </div>
              </template>
              <template #right>
                <Button
                  v-if="!user.isStreamer"
                  label="Create Streamer"
                  @click="emit('createStreamer', user)" />
              </template>
            </ListItem>
            <Divider v-if="index < items.length - 1" />
          </div>
        </template>
        <template #footer>
          <Button
            v-if="authStore.isInAnyRoles(Roles.organizer, Roles.admin)"
            label="Invite New User"
            style="width: 100%"
            @click="emit('newInvite')" />
        </template>
      </DataView>
    </div>
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { Button, Column, DataTable, DataView, Divider } from 'primevue'
import { computed, inject, type Ref } from 'vue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import Roles from '@/constants/roles.ts'
import { useUserStore } from '@/stores/userStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const authStore = useAuthStore()
const userStore = useUserStore()

const usersToDisplay = computed(() =>
  userStore.users.filter((user) => user.registrationDate !== null || user.isAdmin)
)

const emit = defineEmits<{
  newInvite: []
  createStreamer: [UserResponse]
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.users-grid {
  .mobile-info-text {
    display: flex;
    flex-direction: row;
    gap: variables.$space-l;
  }
}
</style>
