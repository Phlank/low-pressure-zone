import { sendGet, sendPost, type QueryParameters } from '../fetchFunctions'
import type { InviteRequest } from './inviteRequest'
import type { LoginRequest } from './loginRequest'
import type { LoginResponse } from './loginResponse'
import type { RegisterRequest } from './registerRequest'
import type { TwoFactorRequest } from './twoFactorRequest'
import type { UserResponse } from './userResponse'
import type { VerifyTokenRequest } from './verifyTokenRequest'

const route = (userId?: string) => '/users' + (userId ? `/${userId}` : '')

export default {
  getInfo: () => sendGet<UserResponse>(`${route()}/info`),
  login: (request: LoginRequest) =>
    sendPost<LoginRequest, LoginResponse>(`${route()}/login`, request),
  logout: () => sendGet<never>(`${route()}/logout`),
  twoFactor: (request: TwoFactorRequest) => sendPost(`${route()}/twofactor`, request),
  invite: (request: InviteRequest) => sendPost(`${route()}/invite`, request),
  resendInvite: (email: string) => sendGet<never>(`${route()}/resendinvite`, { email: email }),
  register: (request: RegisterRequest) => sendPost(`${route()}/register`, request),
  verifyToken: (request: VerifyTokenRequest) => sendGet(`${route()}/verifytoken`, { ...request })
}
