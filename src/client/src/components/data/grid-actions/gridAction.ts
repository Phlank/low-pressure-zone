export interface GridActionEmits {
  create: []
  download: []
  edit: []
  delete: []
  info: []
  resend: []
  clipboard: []
}

export interface GridAction {
  name: string
  icon: string
  severity: 'success' | 'danger' | 'secondary' | 'info'
  emit: keyof GridActionEmits
}

export const gridActions = {
  create: {
    name: 'Create',
    icon: 'pi pi-plus',
    severity: 'success',
    emit: 'create'
  } as GridAction,
  download: {
    name: 'Download',
    icon: 'pi pi-download',
    severity: 'info',
    emit: 'download'
  } as GridAction,
  edit: {
    name: 'Edit',
    icon: 'pi pi-pencil',
    severity: 'secondary',
    emit: 'edit'
  } as GridAction,
  delete: {
    name: 'Delete',
    icon: 'pi pi-trash',
    severity: 'danger',
    emit: 'delete'
  } as GridAction,
  info: {
    name: 'Info',
    icon: 'pi pi-info',
    severity: 'info',
    emit: 'info'
  } as GridAction,
  resend: {
    name: 'Resend',
    icon: 'pi pi-envelope',
    severity: 'secondary',
    emit: 'resend'
  } as GridAction,
  clipboard: {
    name: 'Copy',
    icon: 'pi pi-clipboard',
    severity: 'secondary',
    emit: 'clipboard'
  } as GridAction
}
