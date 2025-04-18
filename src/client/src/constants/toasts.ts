import type { ToastMessageOptions } from 'primevue'

export const noStatsToast: ToastMessageOptions = {
  severity: 'warn',
  summary: 'Unable to load stream metadata',
  detail: 'Stream metadata is currently unavailable, likely due to no stream currently being live.',
  life: 5000,
  closable: true
}
