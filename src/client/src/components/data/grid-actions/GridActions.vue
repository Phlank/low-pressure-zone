<template>
  <div class="grid-actions">
    <div class="grid-actions__buttons">
      <Button
        v-if="!isCombinedIcon"
        v-for="action in visibleActions"
        class="grid-actions__buttons__item"
        :key="action.name"
        :icon="action.icon"
        :severity="action.severity"
        :disabled="props.disabled"
        @click="emit(action.emit as any)"
        rounded
        outlined />
      <Button
        v-else
        class="grid-actions__buttons__item"
        icon="pi pi-ellipsis-h"
        severity="secondary"
        @click="handleCombinedClick"
        :disabled="props.disabled"
        rounded
        outlined />
    </div>
    <GridActionsDrawer
      :visible-actions="visibleActions"
      @action-clicked="handleDrawerActionClick"
      v-model:visible="showActionSelectDrawer" />
  </div>
</template>

<script lang="ts" setup>
import { Button, Divider, Drawer } from 'primevue'
import { computed, inject, ref, type Ref } from 'vue'
import ListItem from '../ListItem.vue'
import { gridActions, type GridAction, type GridActionEmits } from './gridAction'
import GridActionsDrawer from './GridActionsDrawer.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = withDefaults(
  defineProps<{
    showCreate?: boolean
    showEdit?: boolean
    showDelete?: boolean
    showInfo?: boolean
    disabled?: boolean
  }>(),
  {
    showCreate: false,
    showEdit: false,
    showDelete: false,
    showInfo: false,
    disabled: false
  }
)

const emit = defineEmits<GridActionEmits>()

const visibleActions = computed(() => {
  let actions: GridAction[] = []
  if (props.showCreate) actions.push(gridActions.create)
  if (props.showEdit) actions.push(gridActions.edit)
  if (props.showDelete) actions.push(gridActions.delete)
  if (props.showInfo) actions.push(gridActions.info)
  return actions
})

const isCombinedIcon = computed(() => {
  return isMobile?.value && visibleActions.value.length >= 2
})

const showActionSelectDrawer = ref(false)
const handleCombinedClick = () => {
  showActionSelectDrawer.value = true
}

const handleDrawerActionClick = (action: GridAction) => {
  showActionSelectDrawer.value = false
  emit(action.emit as any)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.grid-actions {
  &__buttons {
    width: 100px;
    min-height: 40px;
    text-align: center;

    @include variables.mobile {
      width: 40px;
    }

    &__item {
      margin: 0 variables.$space-s;
      @include variables.mobile {
        margin: variables.$space-s 0;
      }
    }
  }
}
</style>
