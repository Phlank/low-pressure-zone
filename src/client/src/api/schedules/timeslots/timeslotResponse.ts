import type { PerformanceType } from '@/models/performance/performanceType'
import type { PerformerResponse } from '@/api/performers/performerResponse'

export interface TimeslotResponse {
  id: string
  performer: PerformerResponse
  performanceType: PerformanceType
  name?: string
  start: string
  end: string
  createdDate: string
  modifiedDate: string
}
