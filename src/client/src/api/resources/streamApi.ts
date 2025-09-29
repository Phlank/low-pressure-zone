import { sendGet } from '@/api/fetchFunctions.ts'

const route = '/stream'

export default {
  getStatus: () => sendGet<StreamStatusResponse>(`${route}/status`),
  getConnectionInformation: () =>
    sendGet<ConnectionInformationResponse[]>(`${route}/connectioninfo`)
}

export interface StreamStatusResponse {
  isOnline: boolean
  isLive: boolean
  isStatusStale: boolean
  name: string | null
  type: string | null
  listenUrl: string | null
  listenerCount: number
}

export const defaultStreamStatus: StreamStatusResponse = {
  isOnline: false,
  isLive: false,
  isStatusStale: false,
  name: null,
  type: null,
  listenUrl: null,
  listenerCount: 0
}

export interface ConnectionInformationResponse {
  streamType: string
  host: string
  port: string
  mount: string
  username: string
  password: string
  type: string
  displayName: string
}
