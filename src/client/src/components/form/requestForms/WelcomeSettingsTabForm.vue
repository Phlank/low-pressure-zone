<template>
  <FormArea class="welcome-settings-tab-form">
    <IftaFormField
      size="l"
      input-id="titleInput"
      label="Title">
      <InputText
        id="titleInput"
        v-model="state.title"
        @update:model-value="val.validateIfDirty('body')" />
    </IftaFormField>
    <IftaFormField
      size="l"
      input-id="bodyInput"
      label="Body (Accepts Markdown)">
      <Textarea
        auto-resize
        id="bodyInput"
        v-model="state.body"
        @update:model-value="val.validateIfDirty('body')" />
    </IftaFormField>
    <template #actions>
      <slot name="actions"></slot>
    </template>
  </FormArea>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useWelcomeSettingsStore } from '@/stores/settings/welcomeSettingsStore.ts'
import type { TabContent } from '@/api/resources/settingsApi.ts'
import { type Ref, ref } from 'vue'
import { required } from '@/validation/rules/untypedRules.ts'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { InputText, Textarea } from 'primevue'
import { notEmpty } from '@/validation/rules/arrayRules.ts'

const welcomeSettings = useWelcomeSettingsStore()
const props = defineProps<{
  tabTitle?: string
}>()
const tabValue = welcomeSettings.tabs.find((tab) => tab.title === props.tabTitle)
const state: Ref<TabContent> = ref({
  title: tabValue?.title ?? 'New Tab',
  body: tabValue?.body ?? ''
})
const val = createFormValidation(state, {
  title: required(),
  body: required()
})

const reset = () => {
  state.value.title = tabValue?.title ?? 'New Tab'
  state.value.body = tabValue?.body ?? ''
}

const isSubmitting = ref(false)
const submit = async () => {
  if (!val.validate()) return
  isSubmitting.value = true
  const tabs = ref([...welcomeSettings.tabs])
  const tabIndex = tabs.value.findIndex((tab) => tab.title === props.tabTitle)
  if (tabIndex === -1) {
    tabs.value.push(state.value)
  } else {
    tabs.value[tabIndex] = state.value
  }
  const newSetting = ref({ tabs: tabs.value })
  const newSettingValidation = createFormValidation(newSetting, {
    tabs: notEmpty()
  })
  const result = await welcomeSettings.update(newSetting, newSettingValidation)
  isSubmitting.value = false
  if (result.isSuccess) emit('submitted')
}

defineExpose({ isSubmitting, submit, reset, state })

const emit = defineEmits<{
  submitted: []
}>()
</script>
