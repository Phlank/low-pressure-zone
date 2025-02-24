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
        @click="handleActionClick(action.name)"
        rounded
        outlined />
      <Button
        v-if="isCombinedIcon"
        size="small"
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
          @click="handleActionClick(action.name)"
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

const visibleActions = computed(() => {
  let actions: { name: string; icon: string; severity: 'success' | 'danger' | 'secondary' }[] = []
  if (props.showCreate) actions.push({ name: 'Create', icon: 'pi pi-plus', severity: 'success' })
  if (props.showEdit) actions.push({ name: 'Edit', icon: 'pi pi-pencil', severity: 'secondary' })
  if (props.showDelete) actions.push({ name: 'Delete', icon: 'pi pi-trash', severity: 'danger' })
  return actions
})
const isCombinedIcon = computed(() => {
  return visibleActions.value.length >= 2 && isMobile?.value
})

const props = withDefaults(
  defineProps<{
    showCreate?: boolean
    showEdit?: boolean
    showDelete?: boolean
    disabled?: boolean
  }>(),
  {
    showCreate: false,
    showEdit: false,
    showDelete: false,
    disabled: false
  }
)

const showActionSelectDrawer = ref(false)
const handleCombinedClick = () => {
  showActionSelectDrawer.value = true
}

const handleActionClick = (action: string) => {
  showActionSelectDrawer.value = false
  if (action === 'Create') emit('create')
  else if (action === 'Edit') emit('edit')
  else if (action === 'Delete') emit('delete')
}

const emit = defineEmits<{
  create: []
  edit: []
  delete: []
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.grid-actions {
  &__buttons {
    width: 100px;
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
      padding: 20px 0 0 0;
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
}
</style>
