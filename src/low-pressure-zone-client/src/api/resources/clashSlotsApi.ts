import { sendDelete, sendPost, sendPut } from '../fetchFunctions'

const route = (scheduleId: string, id?: string) =>
  `/schedules/${scheduleId}/clash-slots${id ? `/${id}` : ''}`

export default {
  delete: (scheduleId: string) => (id: string) => sendDelete(route(scheduleId, id)),
  post:
    (scheduleId: string) =>
    <TRequest extends ClashSlotRequest>(request: TRequest) =>
      sendPost(route(scheduleId), request),
  put:
    (scheduleId: string) =>
    <TRequest extends ClashSlotRequest>(id: string, request: TRequest) =>
      sendPut(route(scheduleId, id), request)
}

export interface ClashSlotRequest {
  performerOneId: string
  performerTwoId: string
  rounds: string[]
  startsAt: string
  duration: number
}

export interface ClashSlotResponse {
  id: string
  scheduleId: string
  performerOneId: string
  performerTwoId: string
  rounds: string[]
  startsAt: string
  duration: number
  endsAt: string
  isEditable: boolean
  isDeletable: boolean
  type: 'Clash'
}
