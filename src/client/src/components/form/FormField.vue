<template>
  <div :class="computedClass">
    <div>
      <label
        :class="`${baseClass}__label`"
        :for="inputId">
        {{ label }} {{ optional ? '(Optional)' : '' }}
      </label>
      <span :class="`${baseClass}__message`">{{ message ? message : ' ' }}</span>
      <div
        ref="inputDiv"
        :class="`${baseClass}__input`">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, useTemplateRef } from 'vue'

const props = withDefaults(
  defineProps<{
    size: 'xs' | 's' | 'm' | 'l' | 'xl'
    label: string
    message: string
    optional: boolean
    useIftaLabel: boolean
  }>(),
  {
    size: 's',
    message: '',
    optional: false,
    useIftaLabel: false
  }
)

const baseClass = 'form-field'
const widthClass = computed(() => `${baseClass}--${props.size}`)
const computedClass = computed(() => `${baseClass} ${widthClass.value}}`)

const inputId = ref('')
const inputDiv = useTemplateRef('inputDiv')

onMounted(() => {
  setInputId()
})

const setInputId = () => {
  if (!inputDiv.value) {
    inputId.value = ''
    return
  }
  const inputElements = inputDiv.value.getElementsByTagName('input')
  if (inputElements.length === 1) {
    inputId.value = inputElements[0].id
  } else {
    inputId.value = ''
  }
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.form-field {
  display: flex;
  flex-direction: column;
  gap: variables.$space-xs;
  height: min-content;

  &--xs {
    width: min(100%, 1fr);
  }

  &--s {
    width: min(100%, 2fr);
  }

  &--m {
    width: min(100%, 3fr);
  }

  &--l {
    width: min(100%, 4fr);
  }

  &--xl {
    width: min(100%, 5fr);
  }

  &__label {
    font-size: 1rem;
    font-weight: bold;
  }

  &__message {
    width: 100%;
    text-wrap: wrap pretty;
    font-size: 0.8rem;
    color: var(--p-message-error-simple-color);
  }

  &__input {
    width: 100%;

    input {
      width: 100%;
    }
  }
}
</style>
