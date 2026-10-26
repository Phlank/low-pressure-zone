<template>
  <div class="my-account-view">
    <h4>Password</h4>
    <Button
      label="Send Reset Password Email"
      @click="onResetPasswordClick"
      :loading="isPasswordResetSubmitting"
      :disabled="isPasswordResetSubmitting" />
  </div>
</template>

<script setup lang="ts">
import { Button } from 'openvue'
import authApi from '@/api/resources/authApi.ts'
import { ref } from 'vue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const isPasswordResetSubmitting = ref(false)
const onResetPasswordClick = async () => {
  isPasswordResetSubmitting.value = true
  const response = await authApi.getResetPassword()
  isPasswordResetSubmitting.value = false
  tryHandleUnsuccessfulResponse(response)
}
</script>

<style lang="scss"></style>
