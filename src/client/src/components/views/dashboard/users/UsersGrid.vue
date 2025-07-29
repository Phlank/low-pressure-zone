<template>
  <div class="users-grid">
    <div v-if="!isMobile">
      <DataTable
        :rows="10"
        :value="userStore.users"
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
              @click="handleCreateStreamer(data)" />
          </template>
        </Column>
        <template #footer>
          <Button
            v-if="authStore.isInRole(Roles.admin)"
            label="Create Azuracast Streamers"
            @click="showConfirmDialog = true" />
        </template>
      </DataTable>
    </div>
    <div v-else>
      <DataView
        :paginator-templat="mobilePaginatorTemplate"
        :rows="5"
        :value="userStore.users"
        data-key="id"
        paginator>
        <template #list="{ items }: { items: UserResponse[] }">
          <div
            v-for="(user, index) in items"
            :key="user.id">
            <ListItem style="width: 100%">
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
                  @click="handleCreateStreamer(user)" />
              </template>
            </ListItem>
            <Divider v-if="index !== userStore.users.length - 1" />
          </div>
        </template>
        <template #footer>
          <Button
            v-if="authStore.isInRole(Roles.admin)"
            label="Create Azuracast Streamers"
            @click="showConfirmDialog = true" />
        </template>
      </DataView>
    </div>
    <ConfirmDialog
      :is-submitting="isSubmittingConfirmDialog"
      :visible="showConfirmDialog"
      text="Are you sure you want to create streamers for all users without a linked streamer?"
      @close="showConfirmDialog = false"
      @confirm="handleCreateStreamers" />
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { parseDate } from '@/utils/dateUtils'
import { Button, Column, DataTable, DataView, Divider, useToast } from 'primevue'
import { inject, onMounted, ref, type Ref } from 'vue'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { showCreateSuccessToast } from '@/utils/toastUtils.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import ConfirmDialog from '@/components/dialogs/ConfirmDialog.vue'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import Roles from '@/constants/roles.ts'
import { useUserStore } from '@/stores/userStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const toast = useToast()
const authStore = useAuthStore()
const userStore = useUserStore()

const showConfirmDialog = ref(false)
const isSubmittingConfirmDialog = ref(false)

const handleCreateStreamers = async () => {
  isSubmittingConfirmDialog.value = true
  const response = await usersApi.createStreamers()
  if (!tryHandleUnsuccessfulResponse(response, toast)) {
    showCreateSuccessToast(toast, 'Streamers', 'All')
  }
  showConfirmDialog.value = false
  isSubmittingConfirmDialog.value = false
}

const handleCreateStreamer = async (user: UserResponse) => {
  const response = await usersApi.createStreamer(user.id)
  if (!tryHandleUnsuccessfulResponse(response, toast)) {
    showCreateSuccessToast(toast, 'Streamer', user.displayName)
  }
  await userStore.loadUsersAsync()
}

onMounted(async () => {
  await userStore.loadUsersAsync()
})
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
