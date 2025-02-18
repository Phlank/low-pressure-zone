<template>
  <Message class="single-panel-center">You are being logged out.</Message>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import router from '@/router'
import { Message } from 'primevue'
import { Routes } from '@/router/routes'
import { useUserStore } from '@/stores/userStore'
import { onMounted } from 'vue'

const userStore = useUserStore()

onMounted(async () => {
  await userStore.loadIfNotInitialized()
  if (userStore.isLoggedIn()) {
    await api.users.logout.get()
  }
  await userStore.load()
  router.push(Routes.Home)
})
</script>
