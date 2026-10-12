import { sendPostXhr, sendPutXhr } from '@/api/xhrFunctions.ts'
import { sendDelete } from '@/api/fetchFunctions.ts'
import type { Ref } from 'vue'

const route = (scheduleId: string, id?: string) =>
  `/schedules/${scheduleId}/hourlySlots${id ? `/${id}` : ''}`

export const hourlySlotsApi = {
  delete: (scheduleId: string) => (id: string) => sendDelete(route(scheduleId, id)),
  post:
    (scheduleId: string) =>
    <TRequest extends HourlySlotRequest>(request: TRequest, progressRef?: Ref<number>) =>
      sendPostXhr(route(scheduleId), request, progressRef),
  put:
    (scheduleId: string) =>
    <TRequest extends HourlySlotRequest>(id: string, request: TRequest, progressRef?: Ref<number>) =>
      sendPutXhr(route(scheduleId, id), request, progressRef)
}

export interface HourlySlotRequest {
  performerId: string
  subtitle: string | null
  startsAt: string
  duration: number
  replaceMedia: boolean
  deleteMedia: boolean
  file: File | null
}

export interface HourlySlotResponse {
  id: string
  scheduleId: string
  performerId: string
  subtitle: string | null
  startsAt: string
  endsAt: string
  duration: number
  uploadedFileName: string | null
  isPrerecorded: boolean
  isEditable: boolean
  isDeletable: boolean
  type: 'Hourly'
}
