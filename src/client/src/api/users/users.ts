import { sendGet, sendPost } from '../fetchFunctions'
import type { InviteRequest } from './inviteRequest'
import type { LoginRequest } from './loginRequest'
import type { LoginResponse } from './loginResponse'
import type { TwoFactorRequest } from './twoFactorRequest'
import type { UserResponse } from './userResponse'

const route = (userId?: string) => '/users' + (userId ? `/${userId}` : '')

export default {
  getInfo: () => sendGet<UserResponse>(`${route()}/info`),
  login: (request: LoginRequest) =>
    sendPost<LoginRequest, LoginResponse>(`${route()}/login`, request),
  logout: () => sendGet<never>(`${route()}/logout`),
  twoFactor: (request: TwoFactorRequest) => sendPost(`${route()}/twofactor`, request),
  invite: (request: InviteRequest) => sendPost(`${route()}/invite`, request)
}
