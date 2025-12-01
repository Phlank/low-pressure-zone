import { err, ok, type Result } from '@/types/result.ts'
import objectToFormData from '@/utils/objectToFormData.ts'
import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse.ts'

const API_URL = import.meta.env.VITE_API_URL

const sendRequest = async <TRequest extends object, TResponse = never>(
  method: string,
  route: string,
  request?: TRequest | FormData
) => {
  try {
    let formedBody: FormData | string | undefined = undefined
    let isFormData = false
    if (request instanceof FormData) {
      formedBody = request
      isFormData = true
    } else if (request) {
      formedBody = JSON.stringify(request)
    }

    const response = await fetch(`${API_URL}${route}`, {
      body: formedBody,
      method: method,
      headers: request && !isFormData ? { 'Content-Type': 'application/json' } : undefined,
      credentials: 'include'
    })

    const data = response.status === 200 ? ((await response.json()) as TResponse) : undefined
    const validationProblem =
      response.status === 400
        ? ((await response.json()) as ValidationProblemDetails<TRequest>)
        : undefined
    return new ApiResponse<TRequest, TResponse>({
      status: response.status,
      headers: response.headers,
      data: data,
      // Only 400 responses will ever have problem details
      validationProblem: validationProblem,
      error: undefined
    })
  } catch (error: unknown) {
    return new ApiResponse<TRequest, TResponse>({
      status: 0,
      error: error
    })
  }
}

export const sendGet = async <TResponse = never>(route: string, params?: QueryParameters) => {
  if (params) route += toQueryString(params)
  return await sendRequest<never, TResponse>('GET', route)
}

export const sendPut = async <TRequest extends object>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('PUT', route, request)
}

export const sendPutForm = async <TRequest extends object>(route: string, request: TRequest) =>
  sendRequest<TRequest, never>('PUT', route, objectToFormData(request))

export const sendPost = async <TRequest extends object, TResponse = never>(
  route: string,
  request?: TRequest
) => {
  return await sendRequest<TRequest, TResponse>('POST', route, request)
}

export const sendPostForm = <TRequest extends object>(route: string, request: TRequest) => {
  return sendRequest<TRequest, never>('POST', route, objectToFormData(request))
}

export const sendDelete = async (route: string) => {
  return await sendRequest<never, never>('DELETE', route)
}

export const sendDownload = async (route: string): Promise<Result<null, null>> => {
  try {
    const url = `${API_URL}${route}`
    const anchor = document.createElement('a')
    anchor.style.display = 'none'
    anchor.href = url
    anchor.download = 'download'
    document.body.appendChild(anchor)
    anchor.click()
    return ok(null)
  } catch {
    return err(null)
  }
}

export type QueryParameters = Record<string, string | number | boolean | null | undefined>

export const toQueryString = (params: QueryParameters) => {
  const searchParams = new URLSearchParams()
  const keys = Object.keys(params)
  keys.forEach((key) => {
    if (params[key] === null || params[key] === undefined) return
    searchParams.append(key, params[key].toString())
  })
  if (searchParams.size > 0) return `?${searchParams.toString()}`
  return ''
}
