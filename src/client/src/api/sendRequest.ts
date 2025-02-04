import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse'

const API_URL = 'https://localhost:5002'
const LOGIN_URL = 'https://localhost:5002/challenge'

const sendRequest = async <TRequest = never, TResponse = never>(
  method: string,
  route: string,
  request?: TRequest
) => {
  fetch(`${API_URL}${route}`, {
    body: request ? JSON.stringify(request) : null,
    method: method,
    redirect: 'manual'
  }).then(async (response) => {
    if (response.status === 200) {
      return new ApiResponse<TResponse>(response.status, (await response.json()) as TResponse)
    }
    if (response.status === 201) {
      return new ApiResponse<never>(response.status)
    }
    if (response.status === 204) {
      return new ApiResponse<never>(response.status)
    }
    if (response.status === 400) {
      return new ApiResponse<TRequest>(
        response.status,
        undefined,
        (await response.json()) as ValidationProblemDetails<TRequest>
      )
    }
    if (response.status === 401) {
      window.location.href = LOGIN_URL
    }
  })
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
