import { showApiStatusToast } from '@/utils/toastUtils'
import type { FormValidation } from '@/validation/types/formValidation'
import type { ToastServiceMethods } from 'primevue'
import type { ApiResponse } from './apiResponse'

/**
 * Maps validation issues and creates toasts if a response is unsuccessful.
 * @param response The response.
 * @param toast The toast service.
 * @param validation The validation instance. Validation messages will only be mapped if this is specified.
 * @returns `true` if the response is unsuccessful, otherwise `false`.
 */
export const tryHandleUnsuccessfulResponse = <TRequest extends object, TResponse>(
  response: ApiResponse<TRequest, TResponse>,
  toast: ToastServiceMethods,
  validation?: FormValidation<TRequest>
) => {
  if (response.isSuccess()) return false
  if (tryHandleFailure(response, toast)) return true
  if (tryHandleInvalidResponse(response, toast, validation)) return true
  showApiStatusToast(toast, response.status)
  return true
}

const tryHandleInvalidResponse = <TRequest extends object, TResponse>(
  response: ApiResponse<TRequest, TResponse>,
  toast: ToastServiceMethods,
  validation?: FormValidation<TRequest>
): boolean => {
  if (!response.isInvalid()) return false
  if (validation != null) {
    validation.mapApiValidationErrors(response.getValidationErrors())
  }
  const unmappedErrors = response.getValidationErrors()['none'] ?? []
  if (unmappedErrors.length > 0) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: unmappedErrors.join('\n')
    })
  }
  return true
}

const tryHandleFailure = (response: ApiResponse<object, unknown>, toast: ToastServiceMethods) => {
  if (response.isFailure()) {
    toast.add({
      severity: 'error',
      summary: 'HTTP Failure',
      detail: 'There was an issue when sending a request to the API. Is the API operational?'
    })
    return true
  }
  return false
}
