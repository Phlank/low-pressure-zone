<template>
  <Message class="single-panel-center">You are being logged out.</Message>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { Message } from 'primevue'
import { onMounted } from 'vue'

const authStore = useAuthStore()

onMounted(async () => {
  await authStore.loadIfNotInitialized()
  if (authStore.isLoggedIn()) {
    await api.users.logout()
  }
  await authStore.load()
  router.push(Routes.Home)
})
</script>
