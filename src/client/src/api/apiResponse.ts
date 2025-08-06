import httpStatus from '@/constants/httpStatus.ts'

interface ApiResponseParameters<TRequest, TResponse> {
  readonly status: number
  readonly headers?: Headers
  readonly data?: TResponse
  readonly validationProblem?: ValidationProblemDetails<TRequest>
  readonly error?: unknown
}

export class ApiResponse<TRequest, TResponse> {
  readonly status: number
  readonly headers?: Headers
  readonly body?: TResponse
  readonly validationProblem?: ValidationProblemDetails<TRequest>
  readonly error?: unknown

  constructor(parameters: ApiResponseParameters<TRequest, TResponse>) {
    this.status = parameters.status
    this.headers = parameters.headers
    this.body = parameters.data
    this.validationProblem = parameters.validationProblem
    this.error = parameters.error
  }

  /**
   * @returns `true` if the status is successful. If the status is 200, then `data` must be defined.
   */
  readonly isSuccess = () => {
    if (this.status < httpStatus.ok || this.status >= httpStatus.multipleChoices) {
      return false
    }
    if (this.status === httpStatus.ok) {
      return this.body !== undefined
    }
    if (this.status === httpStatus.created) {
      return this.headers?.get('location') !== undefined
    }
    return true
  }

  /**
   * Get the data associated with the request after calling `isSuccess()`
   *
   * @returns `body` if it is not undefined
   * @throws Error if `body is undefined
   */
  readonly data = (): TResponse => {
    if (this.body === undefined) throw new Error('No data in response')
    return this.body
  }

  /**
   * @returns `true` if a validation problem details was returned in the response body.
   */
  readonly isInvalid = () => this.validationProblem !== undefined

  /**
   * @returns Request fields each mapped to an array of error messages.
   */
  readonly getValidationErrors = (): ErrorMessageDictionary<TRequest> =>
    this.validationProblem?.errors ?? {}

  /**
   * @returns `true` if no contact with the API occurred.
   */
  readonly isFailure = () => this.status === 0

  /**
   * @returns the resource id from the location header after a successful POST
   */
  readonly getCreatedId = () => String(this.headers?.get('location')?.split('/').pop())
}

export interface ValidationProblemDetails<TRequest> {
  type: string
  title: string
  status: number
  instance: string
  traceId: string
  errors: ErrorMessageDictionary<TRequest>
}

export type ErrorMessageDictionary<TRequest> = {
  [key in keyof TRequest | 'generalErrors']?: string[]
}
