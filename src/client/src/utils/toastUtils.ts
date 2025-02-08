import { useToast } from 'primevue'

export const showCreateSuccessToast = (entityType: string, entityName?: string) => {
  useToast().add({
    severity: 'success',
    summary: 'Success',
    detail: `Created new ${entityType}${entityName ? ': ' + entityName : ''}`
  })
}
