export const hasIntersection = <T>(a: T[], b: T[]) => {
  if (a.length === 0 || b.length === 0) return false
  for (const item of a) {
    if (b.includes(item)) return true
  }
  return false
}

export const distinct = <T>(array: T[]) => {
  return array.filter((value, index) => {
    return array.indexOf(value) === index
  })
}

export const getEntity = <T extends { id: string }>(array: T[], id: string): T | undefined =>
  array.find((item) => item.id === id)

export const getEntityMap = <T extends { id: string }>(array: T[]) => {
  return array.reduce((map: Record<string, T>, item: T) => {
    map[item.id] = item
    return map
  }, {})
}
