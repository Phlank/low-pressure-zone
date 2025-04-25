<template>
  <a :href="discordInvite">
    <Button
      v-ripple
      color="#7289da"
      icon="pi pi-discord"
      label="Chat" />
  </a>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'

const discordInvite = import.meta.env.VITE_DISCORD_INVITE_LINK
</script>
