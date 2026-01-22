<template>
  <Dialog
    v-bind="$attrs"
    :draggable="false"
    :header="header"
    class="delete-dialog"
    modal
    @hide="emit('hide')"
    @update:visible="isSubmitting ? undefined : emit('hide')">
    <template #default>
      <span v-if="entityName"> Delete {{ entityType }} {{ entityName }}? </span>
      <span v-else> Delete {{ entityType }}? </span>
    </template>
    <template #footer>
      <div class="delete-dialog__footer">
        <Button
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
  isSubmitting: boolean
  entityType: string
  entityName?: string
}>()

const emit = defineEmits<{
  hide: []
  delete: []
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.delete-dialog {
  min-width: min-content;
}
</style>
