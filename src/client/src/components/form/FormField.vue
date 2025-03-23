<template>
  <div :class="computedClass">
    <label
      v-if="label"
      :class="`${baseClass}__label`"
      :for="inputId">
      {{ label }} {{ optional ? '(Optional)' : '' }}
    </label>
    <div :class="`${baseClass}__input`">
      <slot></slot>
    </div>
    <span
      v-if="message !== undefined"
      :class="`${baseClass}__message`">
      {{ message ? message : ' ' }}
    </span>
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import type { FieldSize } from '@/components/form/forms.ts'

const props = withDefaults(
  defineProps<{
    inputId: string
    size?: FieldSize
    label?: string
    message?: string
    optional?: boolean
  }>(),
  {
    size: 's',
    optional: false
  }
)

const baseClass = 'form-field'
const widthClass = computed(() => `${baseClass}--${props.size}`)
const computedClass = computed(() => `${baseClass} ${widthClass.value}`)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.form-field {
  display: flex;
  flex-direction: column;
  gap: variables.$space-s;
  height: min-content;
  margin-bottom: variables.$space-l;

  .p-inputwrapper {
    width: 100%;
  }

  &--xs {
    grid-column: span 2;
  }

  &--s {
    grid-column: span 3;
  }

  &--m {
    grid-column: span 4;
  }

  &--l {
    grid-column: span 6;
  }

  &--xl {
    grid-column: span 8;
  }

  &__label {
    font-size: 1rem;
    font-weight: bold;
    margin-bottom: variables.$space-s;
  }

  &__message {
    width: 100%;
    text-wrap: wrap pretty;
    height: 0.8rem;
    font-size: 0.8rem;
    color: var(--p-red-500);
  }

  &__input {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: start;
    gap: variables.$space-m;

    div {
      display: flex;
      gap: variables.$space-m;
    }
  }
}
</style>
