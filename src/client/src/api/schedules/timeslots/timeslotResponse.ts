import type { PerformerResponse } from '@/api/performers/performerResponse'
import type { PerformanceType } from './performanceType'

export interface TimeslotResponse {
  id: string
  performer: PerformerResponse
  performanceType: PerformanceType
  name: string | null
  start: string
  end: string
}
