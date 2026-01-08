export default function (obj: { [o: string ]: string }): { key: string; value: string }[] {
  return Object.entries(obj).map(([key, value]) => ({ key: key, value: value }))
}
