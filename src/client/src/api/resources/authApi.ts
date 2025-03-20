import { sendGet, sendPost } from '@/api/fetchFunctions.ts'

const route = '/users'

export default {
  getInfo: () => sendGet<UserInfoResponse>(`${route}/info`),
  postLogin: (request: LoginRequest) =>
    sendPost<LoginRequest, LoginResponse>(`${route}/login`, request),
  getLogout: () => sendGet<never>(`${route}/logout`),
  postTwoFactor: (code: string) => sendPost(`${route}/twoFactor`, { code: code }),
  getResetPassword: (email: string) => sendGet(`${route}/resetPassword`, { email: email }),
  postResetPassword: (request: ResetPasswordRequest) => sendPost(`${route}/resetPassword`, request),
  postRegister: (request: RegisterRequest) => sendPost(`${route}/register`, request),
  getVerifyToken: (request: VerifyTokenRequest) => sendGet(`${route}/verifyToken`, { ...request })
}

export interface UserInfoResponse {
  id: string
  username: string
  email: string
  roles: string[]
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  requiresTwoFactor: boolean
}

export interface RegisterRequest {
  context: string
  username: string
  displayName: string
  password: string
  confirmPassword: string
}

export interface ResetPasswordRequest {
  context: string
  password: string
}

export interface VerifyTokenRequest {
  context: string
  purpose: string
  provider: string
}
