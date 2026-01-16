export default function entriesToKeyValueArray(obj: { [o: string ]: string }): { key: string; value: string }[] {
  return Object.entries(obj).map(([key, value]) => ({ key: key, value: value }))
}
