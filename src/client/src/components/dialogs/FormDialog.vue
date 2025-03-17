<template>
  <Dialog
    :draggable="false"
    :header="header"
    :visible="visible"
    class="form-dialog"
    modal
    @hide="emit('close')"
    @show="emit('show')"
    @update:visible="isSubmitting ? undefined : emit('close')">
    <div v-if="visible">
      <slot></slot>
    </div>
    <template #footer>
      <div class="form-dialog__footer">
        <Button
          :disabled="isSubmitting"
          class="input"
          label="Save"
          @click="emit('save')" />
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

<style lang="scss">
@use '@/assets/styles/variables.scss';

.form-dialog {
  min-width: min-content;
  width: min(600px, calc(100vw - 2 * #{variables.$space-l}));
}
</style>
