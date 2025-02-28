<template>
  <div class="linked-performers">
    <div class="user-performers">
      <h4>Your Performers</h4>
      <PerformersGrid
        v-if="linkedPerformers.length > 0"
        :performers="linkedPerformers" />
      <div v-else>
        <p>You do not currently have any linked performers.</p>
        <Button
          @click="emit('toNewPerformer')"
          label="Create Performer" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import type { PerformerResponse } from '@/api/performers/performerResponse'
import { Button } from 'primevue'
import { computed } from 'vue'
import PerformersGrid from './PerformersGrid.vue'

const props = defineProps<{
  performers: PerformerResponse[]
}>()

const linkedPerformers = computed(() => props.performers.filter((performer) => performer.isLinked))

const emit = defineEmits<{
  toNewPerformer: []
}>()
</script>
