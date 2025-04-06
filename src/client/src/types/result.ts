export type Result<Ok, Error> = { isSuccess: boolean; value?: Ok; error?: Error }

export const ok = <T>(value?: T): Result<T, undefined> => {
  return {
    isSuccess: true,
    value: value
  }
}

export const err = <T>(error: T): Result<undefined, T> => {
  return {
    isSuccess: false,
    error: error
  }
}
