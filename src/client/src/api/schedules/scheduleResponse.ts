import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { TimeslotResponse } from '@/api/schedules/timeslots/timeslotResponse'

export interface ScheduleResponse {
  id: string
  start: string
  end: string
  description: string
  audience: AudienceResponse
  timeslots: TimeslotResponse[]
  isEditable: boolean
  isDeletable: boolean
  isTimeslotCreationAllowed: boolean
}
