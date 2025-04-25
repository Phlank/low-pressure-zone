import { sendGet } from '@/api/fetchFunctions.ts'

const route = '/icecast'

export default {
  getStatus: () => sendGet<IcecastStatusResponse>(`${route}/status`)
}

export interface IcecastStatusResponse {
  isOnline: boolean
  isLive: boolean
  isStatusStale: boolean
  name: string | null
  type: string | null
  listenUrl: string | null
}
