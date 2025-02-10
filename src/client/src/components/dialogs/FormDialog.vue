<template>
  <Dialog
    class="form-dialog"
    :visible="visible"
    :header="title"
    @update:visible="isSubmitting ? undefined : emit('close')"
  >
    <template #default><slot></slot></template>
    <template #footer>
      <div class="form-dialog__footer">
        <Button
          class="input"
          label="Cancel"
          @click="emit('close')"
          :disabled="isSubmitting"
          outlined
        />
        <Button class="input" label="Save" @click="emit('save')" :disabled="isSubmitting" />
      </div>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { Dialog, Button } from 'primevue'

defineProps<{
  title: string
  visible: boolean
  isSubmitting: boolean
}>()

const emit = defineEmits<{
  close: []
  save: []
}>()
</script>
