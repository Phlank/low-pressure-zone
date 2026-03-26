const areObjectPropertiesEqual = <T extends object>(a: T, b: T): boolean => {
  const aKeys = Object.keys(a) as (keyof T)[]
  const bKeys = Object.keys(b) as (keyof T)[]

  if (aKeys.length !== bKeys.length) return false

  for (const key of aKeys) {
    if (a[key] !== b[key]) return false
  }

  return true
}

export default areObjectPropertiesEqual
