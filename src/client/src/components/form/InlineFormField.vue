<template>
  <div :class="`${baseClass} ${widthClass}`">
    <div :class="`${baseClass}__field`">
      <div :class="`${baseClass}__field__input`">
        <slot></slot>
      </div>
      <label
        v-if="label"
        :class="`${baseClass}__field__label`"
        :for="inputId">
        {{ label }}
      </label>
    </div>
    <div
      :class="`${baseClass}__message`"
      v-if="message !== undefined">
      {{ message }}
    </div>
  </div>
</template>

<script lang="ts" setup>
import { type FormFieldProps, formFieldPropsDefaults } from '@/components/form/formFieldProps.ts'
import { computed } from 'vue'

const props = withDefaults(defineProps<FormFieldProps>(), formFieldPropsDefaults)
const baseClass = 'inline-form-field'
const widthClass = computed(() => `${baseClass}--${props.size}`)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.inline-form-field {
  display: flex;
  width: 100%;
  flex-direction: column;
  gap: variables.$space-s;
  height: min-content;
  margin-bottom: variables.$space-l;

  &__field {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: variables.$space-m;
    height: min-content;

    &__input {
      display: flex;
      flex-direction: row;
      align-items: center;
    }
  }

  &__message {
    width: 100%;
    text-wrap: wrap pretty;
    line-height: 0.9rem;
    font-size: 0.9rem;
    color: var(--p-red-500);
  }
}
</style>
