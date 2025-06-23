export type Result<Ok, Error> = { isSuccess: boolean; value?: Ok; error?: Error }

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
