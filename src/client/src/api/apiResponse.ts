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

  readonly isSuccess = () => this.status >= 200 && this.status < 300

  readonly isInvalid = () => this.validationProblem != undefined

  readonly getValidationErrors = (): { [key in keyof TRequest | 'none']?: string[] } =>
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
