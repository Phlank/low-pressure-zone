import type { PerformanceType } from '@/models/performance/performanceType'

export interface TimeslotRequest {
  performerId: string
  performanceType: PerformanceType
  name?: string
  start: string
  end: string
}
