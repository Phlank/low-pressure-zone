<template>
  <div class="about-settings-form">
    <FormArea>
      <IftaFormField
        :message="validation.message('bottomText')"
        input-id="topTextInput"
        label="Top"
        size="full">
        <Textarea
          id="topTextInput"
          v-model:model-value="formState.topText"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('topText')"
          auto-resize />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('bottomText')"
        input-id="bottomTextInput"
        label="Bottom"
        size="full">
        <Textarea
          id="bottomTextInput"
          v-model:model-value="formState.bottomText"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('bottomText')"
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
import IftaFormField from '@/components/form/IftaFormField.vue'
import { Button, Textarea, useToast } from 'primevue'
import FormArea from '@/components/form/FormArea.vue'
import { useAboutSettings } from '@/composables/settings/useAboutSettings.ts'
import { ref, type Ref, watch } from 'vue'
import type { AboutSettingsRequest } from '@/api/resources/settingsApi.ts'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { required } from '@/validation/rules/untypedRules.ts'

const toast = useToast()
const aboutSettings = useAboutSettings(toast)

const formState: Ref<AboutSettingsRequest> = ref({
  topText: aboutSettings.topText.value,
  bottomText: aboutSettings.bottomText.value
})
const validation = createFormValidation(formState, {
  topText: required(),
  bottomText: required()
})

watch(
  () => [aboutSettings.topText.value, aboutSettings.bottomText.value],
  () => reset(),
  { once: true }
)

const reset = () => {
  formState.value.topText = aboutSettings.topText.value
  formState.value.bottomText = aboutSettings.bottomText.value
}

const isSubmitting = ref(false)
const submit = async () => {
  isSubmitting.value = true
  await aboutSettings.update(formState, validation)
  isSubmitting.value = false
}

defineExpose({ formState, validation })
</script>

<style lang="scss" scoped></style>
