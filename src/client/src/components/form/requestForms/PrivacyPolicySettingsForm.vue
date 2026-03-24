<template>
  <div class="privacy-policy-settings-form">
    <FormArea is-single-column>
      <IftaFormField
        input-id="privacyPolicyTextInput"
        label="Privacy Policy Text"
        size="full">
        <Textarea
          id="privacyPolicyTextInput"
          v-model:model-value="form.privacyPolicy"
          :invalid="!val.isValid('privacyPolicy')"
          auto-resize />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Button, Textarea } from 'primevue'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { usePrivacyPolicySettingsStore } from '@/stores/settings/privacyPolicySettingsStore.ts'
import type { PrivacyPolicySettingsRequest } from '@/api/resources/settingsApi.ts'
import { ref, type Ref, watch } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { required } from '@/validation/rules/untypedRules.ts'

const privacyPolicySettings = usePrivacyPolicySettingsStore()

const form: Ref<PrivacyPolicySettingsRequest> = ref<PrivacyPolicySettingsRequest>({
  privacyPolicy: privacyPolicySettings.privacyPolicy
})
const val = createFormValidation(form, {
  privacyPolicy: required()
})

watch(
  () => [privacyPolicySettings.privacyPolicy],
  () => reset(),
  { once: true }
)

const reset = () => {
  form.value.privacyPolicy = privacyPolicySettings.privacyPolicy
}

const isSubmitting = ref(false)
const submit = async () => {
  isSubmitting.value = true
  await privacyPolicySettings.update(form, val)
  isSubmitting.value = false
}

defineExpose({ form, val })
</script>
