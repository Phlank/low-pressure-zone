import { sendDownload, sendGet, sendPost, toQueryString } from '@/api/fetchFunctions.ts'

const route = () => `/broadcasts`

export default {
  get: () => sendGet<BroadcastResponse[]>(route()),
  download: (streamerId: number, broadcastId: number) =>
    sendDownload(
      `${route()}/download${toQueryString({
        streamerId: streamerId,
        broadcastId: broadcastId
      })}`
    ),
  disconnect: (request: DisconnectBroadcastRequest) =>
    sendPost<DisconnectBroadcastRequest>(`${route()}/disconnect`, request)
}

export interface BroadcastResponse {
  broadcastId: number
  streamerId: number | null
  streamerDisplayName: string | null
  start: string
  end: string | null
  isDownloadable: boolean
  isDisconnectable: boolean
}

export interface DisconnectBroadcastRequest {
  disableMinutes?: number | null
}
