<template>
  <Dialog
    :draggable="false"
    :header="header"
    class="confirm-dialog"
    modal
    @hide="emit('close')"
    @update:visible="isSubmitting ? undefined : emit('close')">
    <template #default>
      {{ text }}
    </template>
    <template #footer>
      <div class="form-dialog__footer">
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Yes"
          severity="success"
          @click="emit('confirm')" />
      </div>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { Button, Dialog } from 'primevue'

defineProps<{
  isSubmitting: boolean
  text: string
  header?: string
}>()

const emit = defineEmits<{
  close: []
  confirm: []
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.confirm-dialog {
  min-width: min-content;
}
</style>
