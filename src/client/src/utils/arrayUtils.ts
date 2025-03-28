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

export const joinIf = (array: { label: string; condition: boolean }[], separator: string) => {
  const appliedLabels = array.filter((item) => item.condition).map((item) => item.label)
  return appliedLabels.join(separator)
}
