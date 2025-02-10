<template>
  <label :for="props.for">
    <slot></slot>
    <span v-if="optional"> (Optional)</span>
    <span v-if="message"> - {{ message }}</span>
  </label>
</template>

<script lang="ts" setup>
const props = withDefaults(
  defineProps<{
    for: string
    message: string
    optional?: boolean
  }>(),
  {
    optional: false
  }
)
</script>
