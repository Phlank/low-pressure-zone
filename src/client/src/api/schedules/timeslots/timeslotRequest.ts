import type { PerformanceType } from './performanceType'

export interface TimeslotRequest {
  performerId: string
  performanceType: PerformanceType
  name: string | null
  startsAt: string
  endsAt: string
}
