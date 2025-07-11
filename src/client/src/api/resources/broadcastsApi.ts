import { sendDownload, sendGet, toQueryString } from '@/api/fetchFunctions.ts'
import { formatForFilename, parseDate } from '@/utils/dateUtils.ts'

const route = () => `/broadcasts`

export default {
  get: () => sendGet<BroadcastResponse[]>(route()),
  downloadUrl: (broadcast: BroadcastResponse) =>
    `${import.meta.env.VITE_API_URL}${route()}/download${toQueryString({
      streamerId: broadcast.streamerId ?? 0,
      broadcastId: broadcast.broadcastId ?? 0
    })}`,
  download: (broadcast: BroadcastResponse) =>
    sendDownload(
      `${route()}/download${toQueryString({
        streamerId: broadcast.streamerId ?? 0,
        broadcastId: broadcast.broadcastId ?? 0
      })}`,
      `${broadcast.broadcasterDisplayName}_${formatForFilename(parseDate(broadcast.start))}.mp3`
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
