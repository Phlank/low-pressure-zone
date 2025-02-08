<template>
  <label for="for">
    <slot></slot>
    <span v-if="optional"> (Optional)</span>
    <span v-if="message"> - {{ message }}</span>
  </label>
</template>

<script lang="ts" setup>
withDefaults(
  defineProps<{
    for: string
    message: string | null | undefined
    optional?: boolean
  }>(),
  {
    optional: false
  }
)
</script>
