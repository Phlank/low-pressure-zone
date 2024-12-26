<template>
  <div class="navigation">
    <div
      v-for="item in navigationItems"
      :key="item.title"
      class="navigation-item"
      :class="getActiveClass(item)"
      @click="handleNavigationClick(item)"
    >
      <RouterLink :to="item.path">{{ item.title }}</RouterLink>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { setActiveOnTarget as setActiveItem } from '@/utils/arrayUtils'
import { navigationItems, type NavigationItem } from './navigationItem'
import router from '@/router'

const handleNavigationClick = (item: NavigationItem) => {
  setActiveItem(navigationItems.value, 'title', item.title)
  router.push(item.path)
}

const getActiveClass = (navigationItem: NavigationItem) => {
  if (navigationItem.isActive) {
    return 'navigation-item--active'
  }
  return 'navigation-item--inactive'
}
</script>

<style lang="scss" scoped>
@use './../../../assets/variables.scss';

.navigation {
  display: flex;
  justify-content: center;
}

.navigation-item {
  padding: variables.$space-l;
  font-size: large;
  color: variables.$font-color-white-darker;

  &--active {
    a {
      color: variables.$font-color-black-darker;
      text-shadow: none;
      text-decoration: underline;
    }
  }

  &--inactive {
    a {
      color: variables.$font-color-black-brighter;
      text-decoration: none;
    }
  }
}
</style>
