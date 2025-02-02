export class ApiResponse<T> {
  readonly data?: T
  readonly status: number
  private readonly validationProblem?: ValidationProblemDetails

  constructor(
    status: number,
    data: T | undefined = undefined,
    validationProblem: ValidationProblemDetails | undefined = undefined
  ) {
    this.status = status
    this.data = data
    this.validationProblem = validationProblem
  }

  readonly isSuccess = () => this.status < 400 && this.status >= 200

  readonly getValidationErrors = () => this.validationProblem?.errors
}

export interface ValidationProblemDetails {
  type: string
  title: string
  status: number
  instance: string
  traceId: string
  errors: { [key: string]: string[] }
}
