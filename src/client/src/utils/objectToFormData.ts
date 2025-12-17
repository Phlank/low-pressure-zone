const objectToFormData = <T extends object>(
  obj: T,
  form?: FormData,
  parentKey?: string
): FormData => {
  const formData = form || new FormData()

  Object.entries(obj).forEach(([key, value]) => {
    const fieldKey = parentKey ? `${parentKey}[${key}]` : key

    if (value instanceof Blob || value instanceof File) {
      formData.append(fieldKey, value)
      return
    }

    if (Array.isArray(value)) {
      value.forEach((item, idx) => {
        const arrayKey = `${fieldKey}[${idx}]`
        if (typeof item === 'object' && item !== null) {
          objectToFormData(item, formData, arrayKey)
        } else {
          formData.append(arrayKey, String(item))
        }
      })
      return
    }

    if (typeof value === 'object' && value !== null) {
      objectToFormData(value, formData, fieldKey)
      return
    }

    if (value !== undefined && value !== null) {
      formData.append(fieldKey, String(value))
    }
  })

  return formData
}

export default objectToFormData
