export const roles: Record<'admin' | 'organizer' | 'performer', Role> = {
  admin: 'Admin',
  organizer: 'Organizer',
  performer: 'Performer'
}

export type Role = 'Admin' | 'Organizer' | 'Performer'
