import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'
import type { PerformerResponse } from './performersApi.ts'

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

export interface TimeslotRequest {
  performerId: string
  performanceType: PerformanceType
  name: string | null
  startsAt: string
  endsAt: string
}

export interface TimeslotResponse {
  id: string
  performer: PerformerResponse
  performanceType: PerformanceType
  name: string | null
  startsAt: string
  endsAt: string
  isEditable: boolean
  isDeletable: boolean
}

export enum PerformanceType {
  Live = 'Live DJ Set',
  Prerecorded = 'Prerecorded DJ Set'
}

export const performanceTypes = [PerformanceType.Live, PerformanceType.Prerecorded]
