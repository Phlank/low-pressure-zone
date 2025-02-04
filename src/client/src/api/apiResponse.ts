export class ApiResponse<T> {
  readonly data?: T
  readonly status: number
  private readonly validationProblem?: ValidationProblemDetails<T>

  constructor(
    status: number,
    data: T | undefined = undefined,
    validationProblem: ValidationProblemDetails<T> | undefined = undefined
  ) {
    this.status = status
    this.data = data
    this.validationProblem = validationProblem
  }

  readonly isSuccess = () => this.status < 400 && this.status >= 200

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
