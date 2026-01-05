<template>
  <div class="community-form">
    <FormArea>
      <IftaFormField
        :message="val.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="state.name"
          :disabled="isSubmitting"
          :invalid="!val.isValid('name')"
          autofocus
          @update:model-value="val.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('url')"
        input-id="urlInput"
        label="URL"
        size="m">
        <InputText
          id="urlInput"
          v-model:model-value="state.url"
          :disabled="isSubmitting"
          :invalid="!val.isValid('url')"
          @update:model-value="val.validateIfDirty('url')" />
      </IftaFormField>
      <template #actions>
        <slot name="actions"></slot>
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { InputText } from 'primevue'
import { onMounted, ref } from 'vue'
import { communityRequestRules } from '@/validation/requestRules'
import { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useEntityForm } from '@/composables/useEntityForm.ts'

const communities = useCommunityStore()

const props = withDefaults(
  defineProps<{
    community?: CommunityResponse
  }>(),
  {
    community: undefined
  }
)

const { state, val, isSubmitting, submit, reset } = useEntityForm(
  {
    validationRules: communityRequestRules,
    entity: props.community,
    formStateInitializeFn: (community) => {
      return ref({
        name: community?.name ?? '',
        url: community?.url ?? ''
      })
    },
    createPersistentEntityFn: communities.createCommunity,
    updatePersistentEntityFn: communities.updateCommunity,
    onSubmitted: () => emit('submitted')
  }
)

defineExpose({
  formState: state,
  validation: val,
  isSubmitting,
  submit,
  reset
})

const emit = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
