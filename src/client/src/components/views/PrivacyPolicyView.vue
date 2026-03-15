<template>
  <div class="privacy-policy">
    <MarkdownContent :content="privacyPolicyContent" />
  </div>
</template>

<script lang="ts" setup>
import MarkdownContent from '@/components/controls/MarkdownContent.vue'
import {onBeforeMount, ref} from 'vue'
import settingsApi from "@/api/resources/settingsApi.ts";

const privacyPolicyContent = ref('')

onBeforeMount(async () => {
  const response = await settingsApi.getPrivacyPolicySettings()
  if (!response.isSuccess()) {
    privacyPolicyContent.value = 'Unable to load privacy policy content; reload the page or try again later.'
    return
  }

  privacyPolicyContent.value = response.body!.privacyPolicy
})
</script>
