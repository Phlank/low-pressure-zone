<template>
  <div class="form-drawer">
    <Drawer
      class="form-drawer-component"
      :position="position"
      auto-z-index
      modal
      show-close-icon
      v-bind="$attrs">
      <template #header>
        <h2 class="form-drawer__title">{{ title }}</h2>
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

:root {
  --p-form-drawer-content-background: hsl(from var(--p-drawer-background) h s calc(l - 3));
}

div.p-drawer.p-component.form-drawer-component {
  display: flex;
  flex-direction: column;
  flex-grow: unset;
  width: 600px;
  border-radius: variables.$space-m 0 0 variables.$space-m;

  @include variables.mobile() {
    height: fit-content;
    max-height: calc(100dvh - #{variables.$space-l});
    width: 100dvw;
    margin: 0 variables.$space-m;
    border-radius: variables.$space-m variables.$space-m 0 0;
  }

  div.p-drawer-content {
    background-color: var(--p-form-drawer-content-background);
    border-style: solid none solid none;
    border-width: 1px;
    border-color: var(--p-drawer-border-color);
    padding: variables.$space-l;
  }

  div.p-divider-content {
    background-color: var(--p-form-drawer-content-background);
  }

  .form-drawer__title {
    margin: 0;
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
}
</style>
