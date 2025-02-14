import type { PropertyRules } from './propertyRules'

export type RequestValidator<TRequest extends object> =
  | (() => PropertyRules<TRequest>)
  | ((formState: TRequest) => PropertyRules<TRequest>)
