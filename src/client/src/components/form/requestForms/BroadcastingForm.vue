<template>
  <div class="stream-credentials">
    <FormArea>
      <IftaFormField
        input-id="playButtonTextInput"
        label="Play Button Text"
        size="m">
        <InputText
          id="playButtonTextInput"
          v-model="formState.displayName" />
      </IftaFormField>
      <template #actions>
        <Button label="Save" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { Button, InputText } from 'primevue'
import { onMounted, reactive } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'

const connectionInfoStore = useConnectionInfoStore()
const formState = reactive({ displayName: '' })

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
  formState.displayName = connectionInfoStore.liveInfo()?.displayName ?? ''
})
</script>
