import { sendGet, sendPut } from '@/api/fetchFunctions.ts'

const route = '/settings'
const toAbout = '/about'
const toWelcome = '/welcome'

export default {
  getAboutSettings: () => sendGet<AboutSettingsResponse>(route + toAbout),
  putAboutSettings: (request: AboutSettingsRequest) => sendPut(route + toAbout, request),
  getWelcomeSettings: () => sendGet<WelcomeSettingsResponse>(route + toWelcome),
  putWelcomeSettings: (request: WelcomeSettingsRequest) => sendPut(route + toWelcome, request)
}

export type AboutSettingsRequest = AboutSettings
export type AboutSettingsResponse = AboutSettings
interface AboutSettings {
  topText: string
  bottomText: string
}

export type WelcomeSettingsRequest = WelcomeSettings
export type WelcomeSettingsResponse = WelcomeSettings
interface WelcomeSettings {
  tabs: TabContent[]
}

export interface TabContent {
  title: string
  body: string
}
