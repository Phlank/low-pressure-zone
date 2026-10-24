export interface IcecastStats {
  admin: string
  host: string
  location: string
  server_id: string
  server_start: string
  server_start_iso8601: string
  source: IcecastSource | IcecastSource[]
}

export interface IcecastSource {
  artist?: string
  audio_bitrate?: number
  audio_channels?: number
  audio_info?: string
  audio_samplerate?: number
  bitrate?: number
  genre?: string
  'ice-bitrate'?: number
  listener_peak: number
  listeners: number
  listenurl: string
  server_description: string
  server_name: string
  server_type: string
  stream_start: string
  stream_start_iso8601: string
  subtype: string
  title?: string | null
}

export const getLiveSource = (stats?: IcecastStats): IcecastSource | undefined => {
  if (Array.isArray(stats?.source)) {
    return stats?.source.find((sourceItem) => sourceItem.listenurl.endsWith('/live'))
  }
  if (stats?.source.listenurl.endsWith('/live')) {
    return stats.source
  }
  return undefined
}
