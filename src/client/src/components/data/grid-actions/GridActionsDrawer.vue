<template>
  <Drawer
    v-model:visible="visible"
    :show-close-icon="false"
    class="grid-actions-drawer"
    position="bottom">
    <div class="buttons">
      <Button
        v-for="action in visibleActions"
        :key="action.name"
        class="button"
        outlined
        severity="secondary"
        @click="emit('actionClicked', action)">
        <div class="button-content">
          <i :class="action.icon"></i>
          <span>{{ action.name }}</span>
          <i class="pi pi-chevron-right"></i>
        </div>
      </Button>
    </div>
  </Drawer>
</template>

<script lang="ts" setup>
import { Button, Drawer } from 'primevue'
import type { Ref } from 'vue'
import type { GridAction } from './gridAction'

defineProps<{
  visibleActions: GridAction[]
}>()

const visible: Ref<boolean | undefined> = defineModel('visible', { default: false })

const emit = defineEmits<{
  actionClicked: [action: GridAction]
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

div.p-drawer.grid-actions-drawer {
  min-height: fit-content;
  height: auto;
  max-height: 80vh;
}

.grid-actions-drawer {
  border-top-left-radius: variables.$space-l;
  border-top-right-radius: variables.$space-l;

  .p-drawer-header {
    display: none;
  }

  .p-drawer-content {
    padding: variables.$space-l;
  }

  .buttons {
    display: flex;
    flex-direction: column;
    gap: variables.$space-l;

    .button {
      width: 100%;

      .button-content {
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
      }
    }
  }
}
</style>
