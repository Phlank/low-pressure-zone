/* eslint-disable @typescript-eslint/no-unused-vars */
import { invalid, valid } from '@/validation/types/validationResult'

export const alwaysValid = () => () => valid

export const inArray = (arr: unknown[], msg: string) => (arg: unknown) => {
  if (arr.indexOf(arg) > -1) return valid
  return invalid(msg)
}
