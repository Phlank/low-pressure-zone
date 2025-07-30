export default async (text: string): Promise<boolean> => {
  try {
    await navigator.clipboard.writeText(text)
    return true
  } catch (err) {
    console.error('Failed to copy text; will try again with deprecated function: ', err)
    // noinspection JSDeprecatedSymbols
    return document.execCommand('copy')
  }
}
