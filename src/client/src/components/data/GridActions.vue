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
    <Drawer
      class="grid-actions__drawer"
      position="bottom"
      v-model:visible="showActionSelectDrawer"
      :show-close-icon="false">
      <div
        v-for="(action, index) in visibleActions"
        :key="action.name">
        <ListItem
          class="grid-actions__drawer__item"
          @click="handleDrawerActionClick(action.emit)"
          v-ripple>
          <template #left>
            <div class="grid-actions__drawer__item__left">
              <i :class="action.icon"></i>
              <span>{{ action.name }}</span>
            </div>
          </template>
          <template #right>
            <i class="pi pi-chevron-right"></i>
          </template>
        </ListItem>
        <Divider v-if="index < visibleActions.length - 1" />
      </div>
    </Drawer>
  </div>
</template>

<script lang="ts" setup>
import { Button, Divider, Drawer } from 'primevue'
import { computed, inject, ref, type Ref } from 'vue'
import ListItem from './ListItem.vue'

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

interface Emits {
  create: []
  edit: []
  delete: []
  info: []
}

const emit = defineEmits<Emits>()

interface GridAction {
  name: string
  icon: string
  severity: 'success' | 'danger' | 'secondary' | 'info'
  emit: keyof Emits
}

const availableActions: { [key: string]: GridAction } = {
  create: {
    name: 'Create',
    icon: 'pi pi-plus',
    severity: 'success',
    emit: 'create'
  },
  edit: {
    name: 'Edit',
    icon: 'pi pi-pencil',
    severity: 'secondary',
    emit: 'edit'
  },
  delete: {
    name: 'Delete',
    icon: 'pi pi-trash',
    severity: 'danger',
    emit: 'delete'
  },
  info: {
    name: 'Info',
    icon: 'pi pi-info',
    severity: 'info',
    emit: 'info'
  }
}

const visibleActions = computed(() => {
  let actions: GridAction[] = []
  if (props.showCreate) actions.push(availableActions.create)
  if (props.showEdit) actions.push(availableActions.edit)
  if (props.showDelete) actions.push(availableActions.delete)
  if (props.showInfo) actions.push(availableActions.info)
  return actions
})

const isCombinedIcon = computed(() => {
  return isMobile?.value && visibleActions.value.length >= 2
})

const showActionSelectDrawer = ref(false)
const handleCombinedClick = () => {
  showActionSelectDrawer.value = true
}

const handleDrawerActionClick = (emitProperty: keyof Emits) => {
  showActionSelectDrawer.value = false
  emit(emitProperty as any)
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

  &__drawer {
    border-top-left-radius: variables.$space-l;
    border-top-right-radius: variables.$space-l;

    .p-drawer-header {
      display: none;
    }

    .p-drawer-content {
      padding: variables.$space-l;
    }

    &__item {
      padding: variables.$space-m 0;

      &__left {
        display: flex;
        align-items: center;
        gap: variables.$space-l;
      }
    }
  }

  .p-drawer-bottom .p-drawer {
    height: calc(min(80dvh, min-content));
  }
}
</style>
