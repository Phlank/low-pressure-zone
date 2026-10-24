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
  archive: (request: ArchiveBroadcastRequest) =>
    sendPost<ArchiveBroadcastRequest>(`${route()}/archive`, request),
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
  isArchivable: boolean
  isDisconnectable: boolean
}

export interface DisconnectBroadcastRequest {
  disableMinutes?: number | null
}

export interface ArchiveBroadcastRequest {
  id: number
}
