<template>
  <div class="form-drawer">
    <Drawer
      :position="position"
      auto-z-index
      modal
      show-close-icon
      v-bind="$attrs">
      <template #header>
        <h2>{{ title }}</h2>
      </template>
      <slot></slot>
      <template #footer>
        <div class="form-drawer__footer">
          <Button
            class="form-drawer__footer__reset-button"
            label="Reset"
            severity="secondary"
            variant="outlined"
            @click="emit('reset')" />
          <Button
            :disabled="isSubmitting"
            :loading="isSubmitting"
            class="form-drawer__footer__submit-button"
            label="Submit"
            @click="emit('submit')" />
        </div>
      </template>
    </Drawer>
  </div>
</template>

<script lang="ts" setup>
import { Button, Drawer } from 'primevue'
import { computed, inject, ref, type Ref, watch } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const position = computed(() => (isMobile?.value ? 'bottom' : 'right'))

defineProps<{
  isSubmitting: boolean | undefined
  title: string
}>()

const emit = defineEmits(['submit', 'reset'])

const height = ref('100dvh')
watch(
  () => isMobile,
  (newVal) => {
    if (newVal) height.value = '95dvh'
    else height.value = '100dvh'
  },
  { deep: true, immediate: true }
)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

div.p-drawer.p-component {
  display: flex;
  flex-direction: column;
  flex-grow: unset;
  width: 600px;
  border-radius: variables.$space-m 0 0 variables.$space-m;

  @include variables.mobile() {
    height: fit-content;
    max-height: 95dvh;
    width: 100%;
    border-radius: variables.$space-m variables.$space-m 0 0;
  }
}

.form-drawer__footer {
  display: flex;
  justify-content: space-between;
  gap: variables.$space-m;

  &__reset-button,
  &__submit-button {
    flex-grow: 1;
  }
}
</style>
