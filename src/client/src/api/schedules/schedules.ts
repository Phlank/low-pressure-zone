import { sendGet, sendPost, sendPut } from '../fetchFunctions'
import { type ScheduleResponse } from './scheduleResponse'
import type { ScheduleRequest } from './scheduleRequest'

const route = (scheduleId?: string) => `/schedules${scheduleId ? '/' + scheduleId : ''}`

export default {
  get: () => sendGet<ScheduleResponse[]>(route()),
  getById: (id: string) => sendGet<ScheduleResponse>(route(id)),
  post: (request: ScheduleRequest) => sendPost(route(), request),
  put: (id: string, request: ScheduleRequest) => sendPut(route(id), request)
}
