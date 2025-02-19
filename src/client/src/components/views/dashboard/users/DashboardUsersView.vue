<template>
  <div class="dashboard-users">
    <Button
      class="input input--small"
      label="Invite New User"
      @click="showInviteUserDialog = true"
      :disabled="showingDialog" />
    <InviteUserDialog
      :visible="showInviteUserDialog"
      @close="showInviteUserDialog = false" />
  </div>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'
import { computed, ref } from 'vue'
import InviteUserDialog from './InviteUserDialog.vue'

const showingDialog = computed(() => showInviteUserDialog.value)

const showInviteUserDialog = ref(false)
</script>
