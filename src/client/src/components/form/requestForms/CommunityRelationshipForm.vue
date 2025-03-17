<template>
  <div class="community-relationship-form">
    <IftaLabel>
      <Select
        v-if="!props.initialState"
        id="userInput"
        v-model:model-value="formState.user"
        :options="availableUsers"
        optionLabel="username" />
      <label for="userInput">User</label>
    </IftaLabel>
    <div class="community-relationship-form__checkbox">
      <Checkbox
        id="isPerformerInput"
        v-model="formState.isPerformer" />
      <label for="isPerformerInput">Performer</label>
    </div>
    <div class="community-relationship-form__checkbox">
      <Checkbox
        id="isPerformerInput"
        v-model="formState.isOrganizer" />
      <label for="isPerformerInput">Organizer</label>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Checkbox, IftaLabel, Select } from 'primevue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { onMounted, reactive } from 'vue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'

const props = defineProps<{
  initialState?: CommunityRelationshipResponse
  availableUsers: UserResponse[]
}>()

const formState = reactive({
  user: '',
  isPerformer: false,
  isOrganizer: false
})

onMounted(async () => {
  if (props.initialState) {
    formState.user = props.initialState.userId
  }
})
</script>
