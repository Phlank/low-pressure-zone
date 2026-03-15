<template>
  <div class="privacy-policy-tab">
    <div class="privacy-policy-tab__sections">
      <div class="privacy-policy-tab__sections__preview">
        <Divider>Preview</Divider>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Divider } from 'primevue'
import type { PrivacyPolicySettingsRequest } from '@/api/resources/settingsApi.ts'
import { type Ref, ref } from 'vue'
</script>

<style scoped lang="scss"></style>
