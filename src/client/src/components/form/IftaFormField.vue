<template>
  <FormField
    :input-id="inputId"
    :size="size"
    class="ifta-form-field">
    <IftaLabel
      id="iftaLabel"
      class="ifta-form-field__ifta-label">
      <slot></slot>
      <ValidationLabel
        :for="inputId"
        :message="message"
        :optional="optional"
        :text="label" />
    </IftaLabel>
  </FormField>
</template>

<script lang="ts" setup>
import FormField from '@/components/form/FormField.vue'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { IftaLabel } from 'primevue'
import { type FormFieldProps, formFieldPropsDefaults } from '@/components/form/formFieldProps.ts'

withDefaults(defineProps<FormFieldProps>(), formFieldPropsDefaults)
</script>

<style lang="scss">
.ifta-form-field {
  &__ifta-label {
    width: 100%;

    input {
      width: 100%;
    }
  }
}
</style>
