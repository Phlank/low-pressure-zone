import { sendGet, sendPut } from '@/api/fetchFunctions.ts'

const route = '/settings'
const toAbout = '/about'

export default {
  getAboutSettings: () => sendGet<AboutSettingsResponse>(route + toAbout),
  putAboutSettings: (request: AboutSettingsRequest) => sendPut(route + toAbout, request)
}

export type AboutSettingsRequest = AboutSettings
export type AboutSettingsResponse = AboutSettings
interface AboutSettings {
  topText: string
  bottomText: string
}
