import type { PerformanceType } from './performanceType'

export interface TimeslotRequest {
  performerId: string
  performanceType: PerformanceType
  name: string | null
  start: string
  end: string
}
