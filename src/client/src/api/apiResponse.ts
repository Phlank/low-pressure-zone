export class ApiResponse<TRequest, TResponse> {
  readonly data?: TResponse
  readonly status: number
  private readonly validationProblem?: ValidationProblemDetails<TRequest>

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

  readonly getValidationErrors = () => this.validationProblem?.errors
}

export interface ValidationProblemDetails<T> {
  type: string
  title: string
  status: number
  instance: string
  traceId: string
  errors: { [key in keyof T]: string[] }
}
