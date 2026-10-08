export const isNullishOrWhitespace = (value: string | null | undefined): boolean =>
  value === null || value === undefined || value.trim() === ''
