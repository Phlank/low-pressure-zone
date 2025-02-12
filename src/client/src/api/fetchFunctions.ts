import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse'

const API_URL = import.meta.env.VITE_API_URL

const sendRequest = async <TRequest extends object, TResponse = never>(
  method: string,
  route: string,
  request?: TRequest
) => {
  const response = await fetch(`${API_URL}${route}`, {
    body: request ? JSON.stringify(request) : null,
    method: method,
    headers: request ? { 'Content-Type': 'application/json' } : undefined
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
  if (request) {
    return new ApiResponse<TRequest, TResponse>(response.status)
  }
  return new ApiResponse<TRequest, TResponse>(response.status)
}

export const sendGet = async <TResponse>(route: string) => {
  return await sendRequest<never, TResponse>('GET', route)
}

export const sendPut = async <TRequest extends object>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('PUT', route, request)
}

export const sendPost = async <TRequest extends object>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('POST', route, request)
}

export const sendDelete = async (route: string) => {
  return await sendRequest<never, never>('DELETE', route)
}
