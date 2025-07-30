<template>
  <Dialog
    :draggable="false"
    :header="header"
    :visible="visible"
    class="delete-dialog"
    modal
    @hide="emit('close')"
    @update:visible="isSubmitting ? undefined : emit('close')">
    <template #default>
      <span v-if="entityName"> Delete {{ entityType }} {{ entityName }}? </span>
      <span v-else> Delete {{ entityType }}? </span>
    </template>
    <template #footer>
      <div class="delete-dialog__footer">
        <Button
          class="input"
          label="Delete"
          severity="danger"
          @click="emit('delete')" />
      </div>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { Button, Dialog } from 'primevue'

defineProps<{
  header: string
  visible: boolean
  isSubmitting: boolean
  entityType: string
  entityName?: string
}>()

const emit = defineEmits<{
  close: []
  delete: []
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.delete-dialog {
  min-width: min-content;
}
</style>
