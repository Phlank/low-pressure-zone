import { sendGet } from '@/api/fetchFunctions.ts'

const route = '/icecast'

export default {
  getStatus: () => sendGet<IcecastStatus>(`${route}/status`)
}

interface IcecastStatus {
  isOnline: boolean
  name: string | null
  type: string | null
  listenUrl: string | null
}
