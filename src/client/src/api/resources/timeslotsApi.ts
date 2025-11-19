import { sendDelete, sendGet, sendPostForm, sendPut } from '../fetchFunctions'
import type { PerformerResponse } from './performersApi.ts'

const route = (scheduleId: string, timeslotId?: string) =>
  `/schedules/${scheduleId}/timeslots${timeslotId ? '/' + timeslotId : ''}`

export default {
  get: (scheduleId: string) => sendGet<TimeslotResponse[]>(route(scheduleId)),
  getById: (scheduleId: string, timeslotId: string) =>
    sendGet<TimeslotResponse>(route(scheduleId, timeslotId)),
  put: <TRequest extends TimeslotRequest>(
    scheduleId: string,
    timeslotId: string,
    request: TRequest
  ) => sendPut(route(scheduleId, timeslotId), mapRequest(request)),
  post: <TRequest extends TimeslotRequest>(scheduleId: string, request: TRequest) =>
    sendPostForm(route(scheduleId), mapRequest(request)),
  delete: (scheduleId: string, timeslotId: string) => sendDelete(route(scheduleId, timeslotId))
}

export interface TimeslotRequest {
  performerId: string
  performanceType: PerformanceType
  startsAt: string
  endsAt: string
  name: string
  file: File | null
}

const mapRequest = <TRequest extends TimeslotRequest>(request: TRequest): TimeslotRequest => {
  return {
    performerId: request.performerId,
    performanceType: request.performanceType,
    startsAt: request.startsAt,
    endsAt: request.endsAt,
    name: request.name,
    file: request.file
  }
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
