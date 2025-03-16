export class ApiResponse<TRequest extends object, TResponse> {
  readonly data?: TResponse
  readonly status: number
  readonly validationProblem?: ValidationProblemDetails<TRequest>
  readonly error: unknown

  constructor(
    status: number,
    data: TResponse | undefined = undefined,
    validationProblem: ValidationProblemDetails<TRequest> | undefined = undefined,
    error: unknown = undefined
  ) {
    this.status = status
    this.data = data
    this.validationProblem = validationProblem
    this.error = error
  }

  /**
   * @returns `true` if the status is successful. If the status is 200, then `data` must be defined.
   */
  readonly isSuccess = () => {
    const hasSuccessStatus = this.status >= 200 && this.status < 300
    const hasDataIf200 = this.status !== 200 || this.data !== undefined
    return hasSuccessStatus && hasDataIf200
  }

  /**
   * @returns `true` if a validation problem details was returned in the response body.
   */
  readonly isInvalid = () => this.validationProblem != undefined

  /**
   * @returns Request fields each mapped to an array of error messages.
   */
  readonly getValidationErrors = (): ErrorMessageDictionary<TRequest> =>
    this.validationProblem?.errors ?? {}

  /**
   * @returns `true` if no contact with the API occurred.
   */
  readonly isFailure = () => this.status === 0
}

export interface ValidationProblemDetails<TRequest extends object> {
  type: string
  title: string
  status: number
  instance: string
  traceId: string
  errors: ErrorMessageDictionary<TRequest>
}

export type ErrorMessageDictionary<TRequest extends object> = {
  [key in keyof TRequest | 'generalErrors']?: string[]
}
