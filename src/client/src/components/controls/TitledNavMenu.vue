<template>
  <Menubar
    :model="props.items"
    class="titled-nav-menu">
    <template #start>{{ $router.currentRoute.value.name ?? '' }}</template>
    <!-- https://primevue.org/menubar/#router -->
    <template #item="{ item, props, hasSubmenu }">
      <RouterLink
        v-if="item.route"
        v-slot="{ href, navigate }"
        :to="item.route"
        custom>
        <a
          v-ripple
          v-if="item.route"
          :href="href"
          v-bind="props.action"
          @click="navigate">
          <span :class="item.icon" />
          <span>{{ item.label }}</span>
        </a>
        <a
          v-else
          v-ripple
          :href="item.url"
          :target="item.target"
          v-bind="props.action">
          <span :class="item.icon" />
          <span>{{ item.label }}</span>
          <span
            v-if="hasSubmenu"
            class="pi pi-fw pi-angle-down" />
        </a>
      </RouterLink>
    </template>
  </Menubar>
</template>

<script lang="ts" setup>
import { Menubar } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { RouterLink } from 'vue-router'

const props = defineProps<{
  items: MenuItem[]
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.titled-nav-menu {
  a.p-menubar-button {
    margin-left: auto;
  }

  ul.p-menubar-root-list {
    z-index: 99;
  }

  margin-bottom: variables.$space-l;
}
</style>
