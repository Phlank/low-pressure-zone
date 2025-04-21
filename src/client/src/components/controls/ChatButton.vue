<template>
  <Button
    v-ripple
    color="#7289da"
    icon="pi pi-discord"
    label="Chat"
    @click="handleClick" />
</template>

<script lang="ts" setup>
import { Button } from 'primevue'

const discordInvite = import.meta.env.VITE_DISCORD_INVITE_LINK

const handleClick = () => {
  window.location.href = discordInvite
}
</script>
