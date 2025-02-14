export class ApiResponse<TRequest extends object, TResponse> {
  readonly data?: TResponse
  readonly status: number
  readonly validationProblem?: ValidationProblemDetails<TRequest>

  constructor(
    status: number,
    data: TResponse | undefined = undefined,
    validationProblem: ValidationProblemDetails<TRequest> | undefined = undefined
  ) {
    this.status = status
    this.data = data
    this.validationProblem = validationProblem
  }

  readonly isSuccess = () => {
    const hasSuccessStatus = this.status >= 200 && this.status < 300
    const hasDataIf200 = this.status !== 200 || this.data !== undefined
    return hasSuccessStatus && hasDataIf200
  }

  readonly isInvalid = () => this.validationProblem != undefined

  readonly getValidationErrors = (): ErrorMessageDictionary<TRequest> =>
    this.validationProblem?.errors ?? {}
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
  [key in keyof TRequest | 'none']?: string[]
}
