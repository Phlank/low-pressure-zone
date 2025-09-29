export interface GridActionEmits {
  create: []
  download: []
  edit: []
  delete: []
  info: []
  resend: []
}

export interface GridAction {
  name: string
  icon: string
  severity: 'success' | 'danger' | 'secondary' | 'info'
  emit: keyof GridActionEmits
}

export const gridActions = {
  create: {
    performerName: 'Create',
    icon: 'pi pi-plus',
    severity: 'success',
    emit: 'create'
  } as GridAction,
  download: {
    performerName: 'Download',
    icon: 'pi pi-download',
    severity: 'info',
    emit: 'download'
  } as GridAction,
  edit: {
    performerName: 'Edit',
    icon: 'pi pi-pencil',
    severity: 'secondary',
    emit: 'edit'
  } as GridAction,
  delete: {
    performerName: 'Delete',
    icon: 'pi pi-trash',
    severity: 'danger',
    emit: 'delete'
  } as GridAction,
  info: {
    performerName: 'Info',
    icon: 'pi pi-info',
    severity: 'info',
    emit: 'info'
  } as GridAction,
  resend: {
    performerName: 'Resend',
    icon: 'pi pi-envelope',
    severity: 'secondary',
    emit: 'resend'
  } as GridAction
}
