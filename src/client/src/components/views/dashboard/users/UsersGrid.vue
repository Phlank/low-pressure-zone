<template>
  <div class="users-grid">
    <div v-if="!isMobile">
      <DataTable
        :rows="10"
        :value="users"
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
            {{ data.isStreamer ? 'Yes' : '' }}
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
        :value="users"
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
                    <span v-if="user.registrationDate">{{
                      parseDate(user.registrationDate).toLocaleDateString()
                    }}</span>
                  </span>
                </div>
              </template>
            </ListItem>
            <Divider v-if="index !== users.length - 1" />
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
import { inject, ref, type Ref } from 'vue'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { showCreateSuccessToast } from '@/utils/toastUtils.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import ConfirmDialog from '@/components/dialogs/ConfirmDialog.vue'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import Roles from '@/constants/roles.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const toast = useToast()
const authStore = useAuthStore()

defineProps<{
  users: UserResponse[]
}>()

const showConfirmDialog = ref(false)
const isSubmittingConfirmDialog = ref(false)

const handleCreateStreamers = async () => {
  isSubmittingConfirmDialog.value = true
  const response = await usersApi.createStreamers()
  if (response.isSuccess()) {
    showCreateSuccessToast(toast, 'Streamers', 'All')
  } else {
    tryHandleUnsuccessfulResponse(response, toast)
  }
  showConfirmDialog.value = false
  isSubmittingConfirmDialog.value = false
}
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
