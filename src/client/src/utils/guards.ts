export const throwIfUndefined = (obj?: any) => {
  if (obj == undefined) throw new Error('Object was undefined where it must be defined.')
}
