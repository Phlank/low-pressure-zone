import { useToast } from 'primevue/usetoast'

export const showCreateSuccessToast = (entityType: string, entityName?: string) => {
  useToast().add({
    severity: 'success',
    summary: 'Success',
    detail: `Created new ${entityType}${entityName ? ': ' + entityName : ''}`
  })
}

export const showEditSuccessToast = (entityType: string, entityName?: string) => {
  useToast().add({
    severity: 'success',
    summary: 'Success',
    detail: `Updated ${entityType}${entityName ? ': ' + entityName : ''}`
  })
}

export const showApiStatusToast = (status: number) => {
  if (status >= 200 && status < 400) {
    useToast().add({
      severity: 'success',
      summary: 'Success',
      detail: `Action returned with status ${status}`
    })
  } else {
    useToast().add({
      severity: 'error',
      summary: 'Error',
      detail: `Action returned with status ${status}`
    })
  }
}
