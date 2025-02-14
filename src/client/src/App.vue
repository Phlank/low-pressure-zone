<template>
  <div class="app">
    <Toast />
    <SiteLayout />
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed, provide, type ComputedRef, ref, type Ref, onUnmounted } from 'vue'
import SiteLayout from './components/layout/SiteLayout.vue'
import { Toast } from 'primevue'
import { getAuth, onAuthStateChanged, type Auth, type User } from 'firebase/auth'

const screenWidth: Ref<number> = ref(1000)
const isLoggedIn = ref(false)
provide('isLoggedIn', isLoggedIn)

let auth: Auth | undefined
onMounted(() => {
  auth = getAuth()
  onAuthStateChanged(auth, (user: User | null) => {
    if (user) {
      isLoggedIn.value = true
    } else {
      isLoggedIn.value = false
    }
  })
  updateScreenWidth()
  window.addEventListener('resize', updateScreenWidth)
})

onUnmounted(() => {
  window.removeEventListener('resize', updateScreenWidth)
})

const updateScreenWidth = () => {
  screenWidth.value = window.screen.width
}

const mobileWidth = 760
const isMobile: ComputedRef<boolean> = computed(() => screenWidth.value <= mobileWidth)
provide('isMobile', isMobile)
</script>
