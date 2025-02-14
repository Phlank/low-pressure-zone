import { sendDelete, sendGet, sendPost, sendPut } from '../../fetchFunctions'
import type { TimeslotRequest } from './timeslotRequest'
import type { TimeslotResponse } from './timeslotResponse'

const route = (scheduleId: string, timeslotId?: string) =>
  `/schedules/${scheduleId}/timeslots${timeslotId ? '/' + timeslotId : ''}`

export default {
  get: (scheduleId: string) => sendGet<TimeslotResponse[]>(route(scheduleId)),
  getById: (scheduleId: string, timeslotId: string) =>
    sendGet<TimeslotResponse>(route(scheduleId, timeslotId)),
  put: (scheduleId: string, timeslotId: string, request: TimeslotRequest) =>
    sendPut(route(scheduleId, timeslotId), request),
  post: (scheduleId: string, request: TimeslotRequest) => sendPost(route(scheduleId), request),
  delete: (scheduleId: string, timeslotId: string) => sendDelete(route(scheduleId, timeslotId))
}
