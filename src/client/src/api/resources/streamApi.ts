import { sendGet } from '@/api/fetchFunctions.ts'

const route = '/stream'

export default {
  getStatus: () => sendGet<StreamStatusResponse>(`${route}/status`)
}

export interface StreamStatusResponse {
  isOnline: boolean
  isLive: boolean
  isStatusStale: boolean
  name: string | null
  type: string | null
  listenUrl: string | null
}
