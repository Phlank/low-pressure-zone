import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse'

const API_URL = 'https://localhost:5002/api'

const sendRequest = async <TRequest = never, TResponse = never>(
  method: string,
  route: string,
  request?: TRequest
) => {
  const response = await fetch(`${API_URL}${route}`, {
    body: request ? JSON.stringify(request) : null,
    method: method
  })

  if (response.status === 200) {
    return new ApiResponse<never, TResponse>(response.status, (await response.json()) as TResponse)
  }
  if (response.status === 400) {
    return new ApiResponse<TRequest, never>(
      response.status,
      undefined,
      (await response.json()) as ValidationProblemDetails<TRequest>
    )
  }
  return new ApiResponse<never, never>(response.status)
}

export const sendGet = async <TResponse>(route: string) => {
  return await sendRequest<never, TResponse>('GET', route)
}

export const sendPut = async <TRequest>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('PUT', route, request)
}

export const sendPost = async <TRequest>(route: string, request: TRequest) => {
  return await sendRequest<TRequest, never>('POST', route, request)
}

export const sendDelete = async (route: string) => {
  return await sendRequest<never, never>('DELETE', route)
}
