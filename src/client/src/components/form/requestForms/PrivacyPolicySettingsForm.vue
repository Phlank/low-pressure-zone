<template>
  <div class="privacy-policy-form">
    <FormArea
      header="Privacy Policy"
      is-single-column>
      <IftaFormField
        label="Privacy Policy Text"
        input-id="privacyPolicyTextInput"
        size="full">
        <Textarea
          v-model:value="form.privacyPolicy"
          :invalid="!val.isValid('privacyPolicy')"
          id="privacyPolicyTextInput" />
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

<script setup lang="ts">
import { Textarea, Button } from 'primevue'
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
