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
