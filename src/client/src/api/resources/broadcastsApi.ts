import { sendDownload, sendGet, toQueryString } from '@/api/fetchFunctions.ts'

const route = () => `/broadcasts`

export default {
  get: () => sendGet<BroadcastResponse[]>(route()),
  download: (broadcast: BroadcastResponse) =>
    sendDownload(
      `${route()}/download${toQueryString({
        streamerId: broadcast.streamerId ?? 0,
        broadcastId: broadcast.broadcastId ?? 0
      })}`,
      `${broadcast.broadcasterDisplayName}_${encodeURIComponent(broadcast.start)}.mp3`
    )
}

export interface BroadcastResponse {
  broadcastId: number
  streamerId: number | null
  broadcasterDisplayName: string | null
  start: string
  end: string | null
  isDownloadable: boolean
  isDeletable: boolean
  recordingPath: string | null
  nearestPerformerName: string | null
}
