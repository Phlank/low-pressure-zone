<template>
  <Panel class="login single-panel-center">
    <div class="single-panel-center__form">
      <h4>You are being logged out.</h4>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useUserStore } from '@/stores/userStore'
import { Panel } from 'primevue'
import { onMounted } from 'vue'

const userStore = useUserStore()

onMounted(async () => {
  if (await userStore.isLoggedIn()) {
    await api.users.logout.get()
  }
  await userStore.load()
  router.push(Routes.Home)
})
</script>
