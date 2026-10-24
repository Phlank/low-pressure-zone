import type { Entity } from '@/types/entity.ts'
import { parseTime } from '@/utils/dateUtils.ts'

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

export const getEntity = <T extends Entity>(array: T[], id: string): T | undefined =>
  array.find((item) => item.id === id)

export const getEntityMap = <T extends Entity>(array: T[]): Partial<Record<string, T>> => {
  return array.reduce((map: Partial<Record<string, T>>, item: T) => {
    map[item.id] = item
    return map
  }, {})
}

export const entitiesExcept = <T1 extends Entity, T2 extends Entity>(
  source: T1[],
  except: T2[]
): T1[] => {
  if (source.length === 0) return []
  if (except.length === 0) return [...source]
  const output: T1[] = [...source]
  const idsToExclude = except.map((item) => item.id)
  for (const id of idsToExclude) {
    const index = output.findIndex((item) => item.id === id)
    if (index !== -1) {
      output.splice(index, 1)
    }
  }
  return output
}

export const entitiesExceptIds = <T extends Entity>(source: T[], exceptIds: string[]): T[] => {
  if (source.length === 0) return []
  if (exceptIds.length === 0) return [...source]
  const output: T[] = [...source]
  for (const id of exceptIds) {
    const index = output.findIndex((item) => item.id === id)
    if (index !== -1) {
      output.splice(index, 1)
    }
  }
  return output
}

export const includesEntity = <T extends Entity>(array: T[], id: string): boolean =>
  array.some((item) => item.id === id)

// Removes an entity with the specified id from the array.
export const removeEntity = <T extends Entity>(array: T[], id: string): void => {
  const index = array.findIndex((item) => item.id === id)
  if (index === -1) return
  array.splice(index, 1)
}

export const addAlphabetically = <TEntity extends Entity>(
  array: TEntity[],
  entity: TEntity,
  selector: (item: TEntity) => string
): void => {
  const index = array.findIndex(
    (item) => selector(item).toLowerCase().localeCompare(selector(entity).toLowerCase()) > 0
  )
  if (index === -1) {
    array.unshift(entity)
  } else {
    array.splice(index, 0, entity)
  }
}

export const addChronologically = <TEntity extends Entity>(
  array: TEntity[],
  entity: TEntity,
  selector: (item: TEntity) => Date | string
): void => {
  const index = array.findIndex(
    (arrayItem) => parseTime(selector(entity)) < parseTime(selector(arrayItem))
  )
  if (index === -1) {
    array.push(entity)
  } else {
    array.splice(index, 0, entity)
  }
}
