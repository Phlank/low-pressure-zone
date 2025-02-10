<template>
  <Dialog
    :header="header"
    :visible="visible"
    @update:visible="isSubmitting ? undefined : emit('close')"
    :draggable="false"
  >
    <template #default> Delete {{ entityType }} {{ entityName }}? </template>
    <template #footer>
      <div class="form-dialog__footer">
        <Button
          class="input"
          label="Cancel"
          @click="emit('close')"
          :disabled="isSubmitting"
          outlined
        />
        <Button
          class="input"
          severity="danger"
          label="Delete"
          @click="emit('delete')"
          :disabled="isSubmitting"
        />
      </div>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { Dialog, Button } from 'primevue'

defineProps<{
  header: string
  visible: boolean
  isSubmitting: boolean
  entityType: string
  entityName: string
}>()

const emit = defineEmits<{
  close: []
  delete: []
}>()
</script>
