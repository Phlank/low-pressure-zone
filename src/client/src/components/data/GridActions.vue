<template>
  <div class="grid-actions">
    <Button
      v-if="props.showEdit"
      size="small"
      class="grid-actions__item"
      icon="pi pi-pencil"
      severity="secondary"
      @click="emit('edit')"
      :disabled="props.disabled"
      rounded
      outlined />
    <Button
      v-if="props.showDelete"
      size="small"
      class="grid-actions__item"
      icon="pi pi-trash"
      severity="danger"
      @click="emit('delete')"
      :disabled="props.disabled"
      rounded
      outlined />
  </div>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'

const props = withDefaults(
  defineProps<{
    showCreate?: boolean
    showEdit?: boolean
    showDelete?: boolean
    disabled?: boolean
  }>(),
  {
    showCreate: false,
    showEdit: false,
    showDelete: false,
    disabled: false
  }
)

const emit = defineEmits<{
  create: []
  edit: []
  delete: []
}>()
</script>

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.grid-actions {
  width: 6.5rem;
  text-align: center;

  @include variables.mobile {
    width: fit-content;
  }

  .grid-actions__item {
    margin: variables.$space-s;
  }
}
</style>
