<template>
  <div class="schedules-grid">
    <div>
      <DataTable
        :expanded-rows="expandedRows"
        :paginator-template="isMobile ? mobilePaginatorTemplate : undefined"
        :rows="isMobile ? undefined : 10"
        :value="schedules"
        class="schedules-grid__table"
        data-key="id"
        :paginator="!isMobile">
        <template #empty> No items to display.</template>
        <template>
          <Column
            expander
            style="width: fit-content" />
          <Column
            v-if="!isMobile"
            field="community.name"
            header="Community" />
          <Column
            v-if="!isMobile"
            field="start"
            header="Date">
            <template #body="{ data }: { data: ScheduleResponse }">
              {{ parseDate(data.startsAt).toLocaleDateString() }}
            </template>
          </Column>
          <Column
            v-if="!isMobile"
            field="end"
            header="Time">
            <template #body="{ data }: { data: ScheduleResponse }">
              {{ formatReadableTime(parseDate(data.startsAt)) }} -
              {{ formatReadableTime(parseDate(data.endsAt)) }}
            </template>
          </Column>
          <Column v-if="isMobile">
            <template #body="{ data }: { data: ScheduleResponse }">
              <div>{{ parseDate(data.startsAt).toLocaleDateString() }}</div>
              <div class="text-s ellipsis">{{ data.community.name }}</div>
            </template>
          </Column>
          <!-- Only show the action col for grids with schedules in the future -->
          <Column
            v-if="showActionColumn"
            :class="gridActionColClass">
            <template #body="{ data }: { data: ScheduleResponse }">
              <GridActions
                :show-clipboard="
                  communities.isInRelationshipRole(data.community.id, roles.organizer) ||
                  auth.isInRole(roles.admin)
                "
                :show-delete="data.isDeletable"
                :show-edit="data.isEditable"
                @clipboard="handleClipboard(data)"
                @delete="emit('delete', data)"
                @edit="emit('edit', data)" />
            </template>
          </Column>
        </template>
        <template #expansion="rowProps: { data: ScheduleResponse }">
          <SoundclashGrid
            v-if="rowProps.data.type === scheduleTypes.Soundclash"
            :schedule="rowProps.data" />
          <TimeslotsGrid
            v-if="rowProps.data.type === scheduleTypes.Hourly"
            :schedule="rowProps.data" />
        </template>
        <template #paginatorstart>
          <Button
            v-if="!hideActions && !isMobile"
            label="Create Schedule"
            @click="emit('create')" />
        </template>
        <template
          v-if="!hideActions && isMobile"
          #footer>
          <Button
            label="Create Schedule"
            style="width: 100%"
            @click="emit('create')" />
        </template>
      </DataTable>
    </div>
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { formatReadableTime, parseDate, parseTime } from '@/utils/dateUtils'
import { Button, Column, DataTable, useToast } from 'primevue'
import { computed, inject, ref, type Ref } from 'vue'
import TimeslotsGrid from './TimeslotsGrid.vue'
import { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import { scheduleTypes } from '@/constants/scheduleTypes.ts'
import SoundclashGrid from '@/components/views/dashboard/schedules/SoundclashGrid.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { roles } from '@/constants/roles.ts'
import copyToClipboard from '@/utils/copyToClipboard.ts'
import { scheduleToRedditMarkdown } from '@/utils/markdown.ts'
import { useAuthStore } from '@/stores/authStore.ts'

const communities = useCommunityStore()
const auth = useAuthStore()
const toast = useToast()
const expandedRows = ref({})
const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = withDefaults(
  defineProps<{
    hideActions?: boolean
    schedules: ScheduleResponse[]
  }>(),
  {
    hideActions: false
  }
)

const showActionColumn = computed(
  () =>
    !props.hideActions &&
    props.schedules.some(
      (schedule) =>
        parseTime(schedule.endsAt) > Date.now() && (schedule.isDeletable || schedule.isEditable)
    )
)

const emit = defineEmits<{
  create: []
  edit: [schedule: ScheduleResponse]
  delete: [schedule: ScheduleResponse]
}>()

const gridActionColClass = computed(() => {
  return (
    'grid-action-col ' + (isMobile?.value === true ? 'grid-action-col--1' : 'grid-action-col--3')
  )
})

const handleClipboard = (schedule: ScheduleResponse) => {
  const markdown = scheduleToRedditMarkdown(schedule)
  copyToClipboard(markdown)
  toast.add({
    summary: 'Copied to Clipboard',
    detail: 'Schedule markdown has been copied to your clipboard.',
    severity: 'success',
    life: 3000
  })
}
</script>

<style lang="scss">
.schedules-grid {
  &__table {
    .p-datatable-row-expansion > td {
      padding: 0;
    }
  }
}
</style>
