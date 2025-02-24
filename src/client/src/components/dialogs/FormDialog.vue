<template>
  <Dialog
    modal
    class="form-dialog"
    :visible="visible"
    :header="header"
    @update:visible="isSubmitting ? undefined : emit('close')"
    @hide="emit('close')"
    :draggable="false"
    @show="emit('show')">
    <slot></slot>
    <template #footer>
      <div class="form-dialog__footer">
        <Button
          class="input"
          label="Save"
          @click="emit('save')"
          :disabled="isSubmitting" />
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
}>()

const emit = defineEmits<{
  close: []
  save: []
  show: []
}>()
</script>
