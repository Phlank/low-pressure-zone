import { sendGet, sendPost, type QueryParameters } from '../fetchFunctions'
import invites from './invites/invites'
import type { LoginRequest } from './loginRequest'
import type { LoginResponse } from './loginResponse'
import resetPassword from './passwordReset/resetPassword'
import type { RegisterRequest } from './registerRequest'
import type { TwoFactorRequest } from './twoFactorRequest'
import type { UserInfoResponse } from './userInfoResponse'
import type { UserResponse } from './userResponse'
import type { VerifyTokenRequest } from './verifyTokenRequest'

const route = (userId?: string) => '/users' + (userId ? `/${userId}` : '')

export default {
  get: () => sendGet<UserResponse[]>(route()),
  info: () => sendGet<UserInfoResponse>(`${route()}/info`),
  login: (request: LoginRequest) =>
    sendPost<LoginRequest, LoginResponse>(`${route()}/login`, request),
  logout: () => sendGet<never>(`${route()}/logout`),
  twoFactor: (request: TwoFactorRequest) => sendPost(`${route()}/twofactor`, request),
  register: (request: RegisterRequest) => sendPost(`${route()}/register`, request),
  verifyToken: (request: VerifyTokenRequest) => sendGet(`${route()}/verifytoken`, { ...request }),
  resetPassword,
  invites
}
