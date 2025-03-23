<template>
  <Dialog
    :draggable="false"
    :header="header"
    :visible="visible"
    modal
    @hide="emit('close')"
    @update:visible="isSubmitting ? undefined : emit('close')">
    <template #default>
      {{ `Delete ${entityType}${entityName == undefined ? '?' : ' ' + entityName + '?'}` }}
    </template>
    <template #footer>
      <div class="form-dialog__footer">
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
