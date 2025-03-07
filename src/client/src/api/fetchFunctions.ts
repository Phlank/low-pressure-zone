import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse'

const API_URL = import.meta.env.VITE_API_URL

const sendRequest = async <TRequest extends object, TResponse = never>(
  method: string,
  route: string,
  request?: TRequest
) => {
  try {
    const response = await fetch(`${API_URL}${route}`, {
      body: request ? JSON.stringify(request) : null,
      method: method,
      headers: request ? { 'Content-Type': 'application/json' } : undefined,
      credentials: 'include'
    })

    if (response.status === 200) {
      return new ApiResponse<TRequest, TResponse>(
        response.status,
        (await response.json()) as TResponse
      )
    }
    if (response.status === 400) {
      return new ApiResponse<TRequest, TResponse>(
        response.status,
        undefined,
        (await response.json()) as ValidationProblemDetails<TRequest>
      )
    }
    return new ApiResponse<TRequest, TResponse>(response.status)
  } catch (error: any) {
    return new ApiResponse<TRequest, TResponse>(0)
  }
}

export const sendGet = async <TResponse = never>(route: string, params?: QueryParameters) => {
  if (params) route = route + toQueryString(params)
  return await sendRequest<never, TResponse>('GET', route)
}

export const sendPut = async <TRequest extends object>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('PUT', route, request)
}

export const sendPost = async <TRequest extends object, TResponse = never>(
  route: string,
  request: TRequest
) => {
  return await sendRequest<TRequest, TResponse>('POST', route, request)
}

export const sendDelete = async (route: string) => {
  return await sendRequest<never, never>('DELETE', route)
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
