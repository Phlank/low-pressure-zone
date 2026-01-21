<template>
  <div class="test-connection-information">
    <h4>Connection Information</h4>
    <div>
      <ListItem>
        <template #left>Host/Server</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.host }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Port</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.port }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Mount</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.mount }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>User/Login</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.username }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem v-if="!isMobile">
        <template #left>Password</template>
        <template #right>
          <StreamPasswordButton use-type="test" />
        </template>
      </ListItem>
      <div
        class="test-connection-information__mobile-password-button"
        v-else>
        <StreamPasswordButton use-type="test" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Divider } from 'primevue'
import ListItem from '@/components/data/ListItem.vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import StreamPasswordButton from '@/components/controls/StreamPasswordButton.vue'
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const connectionInfoStore = useConnectionInfoStore()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.test-connection-information {
  &__mobile-password-button {
    margin: variables.$space-m;
    display: flex;
    justify-content: center;
  }
}
</style>
