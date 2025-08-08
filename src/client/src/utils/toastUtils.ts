import type { ToastServiceMethods } from 'primevue'

const TOAST_DURATION = 5000

export const showSuccessToast = (
  toast: ToastServiceMethods,
  action: 'Created' | 'Updated' | 'Deleted' | 'Registered',
  entityType: string,
  entityName?: string
) => {
  toast.add({
    severity: 'success',
    summary: 'Success',
    detail: `${action} ${entityType}${entityName ? ': ' + entityName : ''}`,
    life: TOAST_DURATION
  })
}

export const showCreateSuccessToast = (
  toast: ToastServiceMethods,
  entityType: string,
  entityName?: string
) => {
  toast.add({
    severity: 'success',
    summary: 'Success',
    detail: `Created new ${entityType}${entityName ? ': ' + entityName : ''}`,
    life: TOAST_DURATION
  })
}

export const showEditSuccessToast = (
  toast: ToastServiceMethods,
  entityType: string,
  entityName?: string
) => {
  toast.add({
    severity: 'success',
    summary: 'Success',
    detail: `Updated ${entityType}${entityName ? ': ' + entityName : ''}`,
    life: TOAST_DURATION
  })
}

export const showDeleteSuccessToast = (
  toast: ToastServiceMethods,
  entityType: string,
  entityName?: string
) => {
  toast.add({
    severity: 'success',
    summary: 'Success',
    detail: `Deleted ${entityType}${entityName ? ': ' + entityName : ''}`,
    life: TOAST_DURATION
  })
}

export const showApiStatusToast = (toast: ToastServiceMethods, status: number) => {
  if (status >= 200 && status < 400) {
    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: `Action returned with status ${status}`,
      life: TOAST_DURATION
    })
  } else {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: `Action returned with status ${status}`,
      life: TOAST_DURATION
    })
  }
}

export const showErrorToast = (toast: ToastServiceMethods, error: string) => {
  toast.add({
    severity: 'error',
    summary: 'Error',
    detail: error,
    life: TOAST_DURATION
  })
}
