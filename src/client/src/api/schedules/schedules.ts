import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'
import { type ScheduleResponse } from './scheduleResponse'
import type { ScheduleRequest } from './scheduleRequest'

const route = (scheduleId?: string) => `/schedules${scheduleId ? '/' + scheduleId : ''}`

export default {
  get: (params?: { before?: string; after?: string }) =>
    sendGet<ScheduleResponse[]>(route(), params),
  getById: (id: string) => sendGet<ScheduleResponse>(route(id)),
  post: <TSchedule extends ScheduleRequest>(request: TSchedule) =>
    sendPost<ScheduleRequest>(route(), mapRequest(request)),
  put: <TSchedule extends ScheduleRequest>(id: string, request: TSchedule) =>
    sendPut<ScheduleRequest>(route(id), mapRequest(request)),
  delete: (id: string) => sendDelete(route(id))
}

// Forms will deal with dates, but we don't want to send the actual dates into the API
const mapRequest = <TSchedule extends ScheduleRequest>(schedule: TSchedule): ScheduleRequest => {
  return { audienceId: schedule.audienceId, start: schedule.start, end: schedule.end }
}
