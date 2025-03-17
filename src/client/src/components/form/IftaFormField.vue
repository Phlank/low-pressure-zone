<template>
  <FormField
    :input-id="inputId"
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
import type { FieldSize } from '@/components/form/forms.ts'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { IftaLabel } from 'primevue'

withDefaults(
  defineProps<{
    size?: FieldSize
    label: string
    inputId: string
    message?: string
    optional?: boolean
  }>(),
  {
    size: 's',
    message: ''
  }
)
</script>

<style lang="scss">
.ifta-form-field {
  &__ifta-label {
    width: 100%;
  }
}
</style>
