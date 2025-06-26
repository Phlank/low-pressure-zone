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

export interface ConnectionInformationResponse {
  streamType: string
  host: string
  port: string
  username: string
  password: string
  type: string
  streamTitleField: string
}
