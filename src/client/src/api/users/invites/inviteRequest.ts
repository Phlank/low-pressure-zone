import type { Role } from '@/constants/roles'

export interface InviteRequest {
  email: string
  role: Role
}
