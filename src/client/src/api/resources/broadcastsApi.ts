import { sendDownload, sendGet, toQueryString } from '@/api/fetchFunctions.ts'

const route = () => `/broadcasts`

export default {
  get: () => sendGet<BroadcastResponse[]>(route()),
  download: (streamerId: number, broadcastId: number) =>
    sendDownload(
      `${route()}/download${toQueryString({
        streamerId: streamerId ?? 0,
        broadcastId: broadcastId ?? 0
      })}`
    )
}

export interface BroadcastResponse {
  broadcastId: number
  streamerId: number | null
  streamerDisplayName: string | null
  start: string
  end: string | null
  isDownloadable: boolean
  isDeletable: boolean
}
