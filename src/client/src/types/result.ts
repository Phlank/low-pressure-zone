export type Result<T, TError> = {
  isSuccess: boolean
  value?: T
  error?: TError
}

export const ok = <T, TErr>(value?: T): Result<T, TErr> => {
  return {
    isSuccess: true,
    value: value
  }
}

export const err = <T, TErr>(error: TErr): Result<T, TErr> => {
  return {
    isSuccess: false,
    error: error
  }
}
