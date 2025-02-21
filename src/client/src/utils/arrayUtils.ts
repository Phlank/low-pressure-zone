/**
 * Loops through an array, marking the first item where a specified property equals a specified value as active, and leaving the rest inactive.
 * @param array
 * @param targetProperty
 * @param targetValue
 */
export const setActiveOnTarget = <T extends { isActive: boolean }, TProperty>(
  array: T[],
  targetProperty: keyof T,
  targetValue: TProperty
) => {
  let targetIsActive = false
  array.forEach((item) => {
    if (!targetIsActive && item[targetProperty] == targetValue) {
      item.isActive = true
      targetIsActive = true
      return
    }
    item.isActive = false
  })
}

export const intersection = <T>(a: T[], b: T[]) => {
  const output: T[] = []
  if (a.length === 0 || b.length === 0) return output
  a.forEach((itemA) => {
    if (b.includes(itemA)) output.push(itemA)
  })
  return output
}

export const hasIntersection = <T>(a: T[], b: T[]) => {
  if (a.length === 0 || b.length === 0) return false
  for (let i = 0; i < a.length; i++) {
    if (b.includes(a[i])) return true
  }
  return true
}

export const distinct = <T>(array: T[]) => {
  return array.filter((value, index) => {
    return array.indexOf(value) === index
  })
}
