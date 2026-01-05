import { sendDelete, sendGet } from '../fetchFunctions'
import type { PerformerResponse } from './performersApi.ts'
import { sendPostXhr, sendPutXhr } from '@/api/xhrFunctions.ts'
import type { Ref } from 'vue'

const route = (id?: string) => `/timeslots${id ? '/' + id : ''}`

export default {
  get: (scheduleId: string) => sendGet<TimeslotResponse[]>(route(), { scheduleId: scheduleId }),
  getById: (timeslotId: string) => sendGet<TimeslotResponse>(route(timeslotId)),
  put: <TRequest extends TimeslotRequest>(
    timeslotId: string,
    request: TRequest,
    progressRef?: Ref<number>
  ) => sendPutXhr(route(timeslotId), mapRequest(request), progressRef),
  post: <TRequest extends TimeslotRequest>(request: TRequest, progressRef: Ref<number> | undefined) =>
    sendPostXhr(route(), mapRequest(request), progressRef),
  delete: (timeslotId: string) => sendDelete(route(timeslotId))
}

export interface TimeslotRequest {
  scheduleId: string
  performerId: string
  performanceType: PerformanceType
  startsAt: string
  endsAt: string
  name: string
  replaceMedia: boolean | null
  file: File | null
}

const mapRequest = <TRequest extends TimeslotRequest>(request: TRequest): TimeslotRequest => {
  return {
    scheduleId: request.scheduleId,
    performerId: request.performerId,
    performanceType: request.performanceType,
    startsAt: request.startsAt,
    endsAt: request.endsAt,
    name: request.name,
    file: request.file,
    replaceMedia: request.replaceMedia
  }
}

export interface TimeslotResponse {
  id: string
  scheduleId: string
  performer: PerformerResponse
  performanceType: PerformanceType
  name: string | null
  startsAt: string
  endsAt: string
  isEditable: boolean
  isDeletable: boolean
  uploadedFileName: string | null
}

export enum PerformanceType {
  Live = 'Live DJ Set',
  Prerecorded = 'Prerecorded DJ Set'
}

export const performanceTypes = [PerformanceType.Live, PerformanceType.Prerecorded]
