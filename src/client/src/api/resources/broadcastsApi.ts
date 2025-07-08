import { sendGet } from '@/api/fetchFunctions.ts'

const route = () => `/broadcasts`

export default {
  get: () => sendGet<BroadcastResponse[]>(route())
}

export interface BroadcastResponse {
  broadcastId: number
  streamerId: number | null
  broadcasterDisplayName: string | null
  start: string
  end: string | null
  isDownloadable: boolean
  recordingPath: string | null
  nearestPerformerName: string | null
}
