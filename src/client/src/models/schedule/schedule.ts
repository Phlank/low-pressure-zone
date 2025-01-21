import type { Timeslot } from './timeslot'

export interface Schedule {
  startDate: Date
  audience: string
  description: string
  timeslots: Timeslot[]
}
