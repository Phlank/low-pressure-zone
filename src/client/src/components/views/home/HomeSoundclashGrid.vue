<template>
  <div class="home-soundclash-grid">
    <DataTable
      :value="rows"
      :loading="schedules.isLoading">
      <template #empty> No soundclashes have been scheduled yet. </template>
      <Column
        field="start"
        header="Time">
        <template #body="{ data }: { data: SoundclashRow }">
          <SlotTime :date="data.start" />
        </template>
      </Column>
      <Column
        field="soundclash"
        style="width: 100%">
        <template #body="{ data }: { data: SoundclashRow }">
          <div
            v-if="data.soundclash"
            class="home-soundclash-grid__slot">
            <div class="home-soundclash-grid__slot__performers">
              <div class="home-soundclash-grid__slot__performers__name">
                {{ data.soundclash.performerOne.name }}
              </div>
              <Divider type="dotted"><i>vs.</i></Divider>
              <div class="home-soundclash-grid__slot__performers__name">
                {{ data.soundclash.performerTwo.name }}
              </div>
            </div>
            <Divider
              v-if="isMobile"
              type="dotted" />
            <div class="home-soundclash-grid__slot__rounds">
              <div
                v-for="round in [
                  data.soundclash.roundOne,
                  data.soundclash.roundTwo,
                  data.soundclash.roundThree
                ]"
                :key="round">
                {{ round }}
              </div>
            </div>
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import type { SoundclashResponse } from '@/api/resources/soundclashApi'
import { computed, inject, ref, type Ref, watch } from 'vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { parseDate, timesBetween } from '@/utils/dateUtils.ts'
import { Column, DataTable, Divider } from 'primevue'
import SlotTime from '@/components/controls/SlotTime.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const schedules = useScheduleStore()

const props = defineProps<{
  scheduleId: string
}>()

const schedule = computed(() => schedules.getScheduleById(props.scheduleId))
interface SoundclashRow {
  start: Date
  soundclash?: SoundclashResponse
}
const rows: Ref<SoundclashRow[]> = ref([])
const setupRows = () => {
  const newRows: SoundclashRow[] = []
  if (!schedule.value || schedule.value.soundclashes.length === 0) {
    rows.value = newRows
    return
  }
  const soundclashes = schedule.value.soundclashes
  const startFirst = new Date(soundclashes[0]!.startsAt)
  const endLast = new Date(soundclashes[schedule.value.soundclashes.length - 1]!.endsAt)
  const hours = timesBetween(startFirst, endLast, 120)

  hours.forEach((hour) => {
    const soundclash = soundclashes.find(
      (sc) => parseDate(sc.startsAt).getTime() === hour.getTime()
    )
    newRows.push({
      start: hour,
      soundclash: soundclash
    })
  })
  rows.value = newRows
}

watch(
  () => schedule.value,
  () => {
    setupRows()
  },
  { immediate: true, deep: true }
)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.home-soundclash-grid {
  &__slot {
    display: flex;
    justify-content: space-between;
    align-items: center;

    @include variables.mobile() {
      flex-direction: column;
    }

    &__performers {
      display: flex;
      flex-direction: column;
      align-items: center;
      width: 100%;
      margin: variables.$space-s 0;
      text-align: center;
      &__name {
        font-weight: bolder;
        font-size: large;
      }
    }

    &__rounds {
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: variables.$space-m;
      font-weight: 500;
    }
  }
}
</style>
