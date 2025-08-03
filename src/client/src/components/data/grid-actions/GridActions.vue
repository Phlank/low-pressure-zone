<template>
  <div class="grid-actions">
    <div class="grid-actions__buttons">
      <div
        v-if="!isCombinedIcon"
        class="grid-actions__buttons__multi">
        <div
          v-for="action in visibleActions"
          :key="action.name">
          <Button
            :disabled="props.disabled"
            :icon="action.icon"
            :severity="action.severity"
            class="grid-actions__buttons__item"
            outlined
            rounded
            @click="emit(action.emit as any)" />
        </div>
      </div>
      <Button
        v-else
        :disabled="props.disabled"
        class="grid-actions__buttons__item"
        icon="pi pi-ellipsis-h"
        outlined
        rounded
        severity="secondary"
        @click="handleCombinedClick" />
    </div>
    <GridActionsDrawer
      v-model:visible="showActionSelectDrawer"
      :visible-actions="visibleActions"
      @action-clicked="handleDrawerActionClick" />
  </div>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'
import { computed, inject, ref, type Ref } from 'vue'
import { type GridAction, type GridActionEmits, gridActions } from './gridAction'
import GridActionsDrawer from './GridActionsDrawer.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = withDefaults(
  defineProps<{
    showCreate?: boolean
    showDownload?: boolean
    showEdit?: boolean
    showDelete?: boolean
    showInfo?: boolean
    showResend?: boolean
    disabled?: boolean
  }>(),
  {
    showCreate: false,
    showDownload: false,
    showEdit: false,
    showDelete: false,
    showInfo: false,
    showResend: false,
    disabled: false
  }
)

const emit = defineEmits<GridActionEmits>()

const visibleActions = computed(() => {
  const actions: GridAction[] = []
  if (props.showCreate) actions.push(gridActions.create)
  if (props.showDownload) actions.push(gridActions.download)
  if (props.showEdit) actions.push(gridActions.edit)
  if (props.showDelete) actions.push(gridActions.delete)
  if (props.showInfo) actions.push(gridActions.info)
  if (props.showResend) actions.push(gridActions.resend)
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
  emit(action.emit as never)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.grid-actions {
  &__buttons {
    width: 100px;
    min-height: 40px;
    text-align: center;

    &__multi {
      display: flex;
      flex-direction: row;
      justify-content: center;
    }

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
