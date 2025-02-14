<template>
  <label :for="props.for">
    <slot>{{ text }}</slot>
    <span v-if="optional"> (Optional)</span>
    <span v-show="message"> - {{ message }}</span>
  </label>
</template>

<script lang="ts" setup>
const props = withDefaults(
  defineProps<{
    for: string
    text?: string
    message: string
    optional?: boolean
  }>(),
  {
    optional: false
  }
)
</script>
