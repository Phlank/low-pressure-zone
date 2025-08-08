<template>
  <div class="stream-information">
    <div class="stream-information__live">
      <h2>Live Stream</h2>
      <h4>Broadcasting</h4>
      <BroadcastingForm
        v-if="connectionInfoStore.liveInfo()"
        :info="connectionInfoStore.liveInfo()!"
        class="stream-information__live__broadcasting-form" />
      <LiveConnectionInformation />
    </div>
    <div class="stream-information__test">
      <h2>Test Stream</h2>
      <TestConnectionInformation />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import BroadcastingForm from '@/components/form/requestForms/BroadcastingForm.vue'
import LiveConnectionInformation from '@/components/views/dashboard/streaming/LiveConnectionInformation.vue'
import TestConnectionInformation from '@/components/views/dashboard/streaming/TestConnectionInformation.vue'
import { onMounted } from 'vue'

const connectionInfoStore = useConnectionInfoStore()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.stream-information {
  &__live {
    display: flex;
    flex-direction: column;

    &__broadcasting-form {
      margin-bottom: variables.$space-l;
    }
  }

  &__test {
    display: flex;
    flex-direction: column;
  }

  &__modal-footer {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: variables.$space-m;
  }
}
</style>
