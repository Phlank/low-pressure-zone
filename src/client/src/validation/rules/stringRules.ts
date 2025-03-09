import { distinct } from '@/utils/arrayUtils'
import { invalid, valid } from '@/validation/types/validationResult'
import { combineRules, type ValidationRule } from '@/validation/types/validationRule'

export const required =
  (msg?: string): ValidationRule<string> =>
  (arg) => {
    if (arg.trim().length == 0) return invalid(msg ?? 'Required field')
    return valid
  }

export const url =
  (msg?: string): ValidationRule<string> =>
  (arg) => {
    if (arg.trim().length == 0) return valid
    if (URL.parse(arg)) return valid
    return invalid(msg ?? 'Invalid URL')
  }

const emailRegex =
  /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/

export const emailAddress =
  (msg?: string): ValidationRule<string> =>
  (arg) => {
    if (arg.trim().length == 0) return valid
    if (emailRegex.test(arg)) return valid
    return invalid(msg ?? 'Invalid email')
  }

export const equals =
  (selector: () => string, msg?: string): ValidationRule<string> =>
  (arg: string) => {
    if (arg === selector()) return valid
    return invalid(msg ?? 'Does not match')
  }

export const allowedCharacters =
  (charSet: string): ValidationRule<string> =>
  (arg: string) => {
    const regex = new RegExp(`[^${charSet}]`)
    const matches = regex.exec(arg)
    if (!matches) return valid
    var characters = distinct(matches.map((match) => match))
    return invalid(`Invalid characters: ${characters.join(' ')}`)
  }

export const requireAnyCharacter =
  (charSet: string, msg: string): ValidationRule<string> =>
  (arg: string) => {
    const regex = new RegExp(`[${charSet}]`)
    if (regex.test(arg)) return valid
    return invalid(msg)
  }

export const requireAnyOtherCharacter =
  (charSet: string, msg: string): ValidationRule<string> =>
  (arg: string) => {
    const regex = new RegExp(`[^${charSet}]`)
    if (regex.test(arg)) return valid
    return invalid(msg)
  }

export const minimumLength = (length: number, msg?: string) => (arg: string) => {
  if (arg.length < length) return invalid(msg ?? `Minimum ${length} characters`)
  return valid
}

export const password = combineRules(
  minimumLength(8),
  requireAnyCharacter('A-Z', 'Requires uppercase'),
  requireAnyCharacter('a-z', 'Requires lowercase'),
  requireAnyCharacter('0-9', 'Requires number'),
  requireAnyOtherCharacter('A-Za-z0-9', 'Requires symbol')
)
