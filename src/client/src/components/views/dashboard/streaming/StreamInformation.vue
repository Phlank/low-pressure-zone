<template>
  <div class="stream-information">
    <h3>Live Stream</h3>
    <DataView
      v-if="connectionInfoStore.liveInfo() !== undefined"
      :value="liveItems"
      class="stream-information__live">
      <template #list="slotProps">
        <div
          v-for="(item, index) in slotProps.items as LabeledField[]"
          :key="index">
          <ListItem>
            <template #left>{{ item.label }}</template>
            <template #right>{{ item.value }}</template>
          </ListItem>
          <Divider
            v-if="index < slotProps.items.length - 1"
            style="margin: 0" />
        </div>
      </template>
    </DataView>
    <h3>Test Stream</h3>
    <DataView
      v-if="connectionInfoStore.testInfo() !== undefined"
      :value="testItems"
      class="stream-information__live">
      <template #list="slotProps">
        <div
          v-for="(item, index) in slotProps.items as LabeledField[]"
          :key="index">
          <ListItem>
            <template #left>{{ item.label }}</template>
            <template #right>{{ item.value }}</template>
          </ListItem>
          <Divider
            v-if="index < slotProps.items.length - 1"
            style="margin: 0" />
        </div>
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { onMounted, ref } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { DataView, Divider } from 'primevue'
import type { LabeledField } from '@/types/labeledField.ts'

const connectionInfoStore = useConnectionInfoStore()
const liveItems = ref<LabeledField[]>([])
const testItems = ref<LabeledField[]>([])

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
  const liveInfo = connectionInfoStore.liveInfo()!
  liveItems.value.push({ label: 'Host/Server', value: liveInfo.host })
  liveItems.value.push({ label: 'Port', value: liveInfo.port })
  liveItems.value.push({ label: 'Username', value: liveInfo.username })
  liveItems.value.push({ label: 'Password', value: liveInfo.password })
  liveItems.value.push({ label: 'Stream Title Field', value: liveInfo.streamTitleField })
  const testInfo = connectionInfoStore.testInfo()!
  testItems.value.push({ label: 'Host/Server', value: testInfo.host })
  testItems.value.push({ label: 'Port', value: testInfo.port })
  testItems.value.push({ label: 'Username', value: testInfo.username })
  testItems.value.push({ label: 'Password', value: testInfo.password })
  testItems.value.push({ label: 'Stream Title Field', value: testInfo.streamTitleField })
})
</script>

<style lang="scss" scoped></style>
