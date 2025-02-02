import { sendDelete, sendPost, sendPut } from '@/api/axiosInstance'
import type { TimeslotRequest } from './timeslotRequest'

const route = (scheduleId: string, timeslotId?: string) =>
  `/schedules/${scheduleId}/timeslots${timeslotId ? '/' + timeslotId : ''}`

export default {
  put: (scheduleId: string, timeslotId: string, request: TimeslotRequest) => () =>
    sendPut(route(scheduleId, timeslotId), request),
  post: (scheduleId: string, request: TimeslotRequest) => () =>
    sendPost(route(scheduleId), request),
  delete: (scheduleId: string, timeslotId: string) => () =>
    sendDelete(route(scheduleId, timeslotId))
}
