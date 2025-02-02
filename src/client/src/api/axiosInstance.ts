import axios, { AxiosError } from 'axios'
import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse'

const axiosInstance = axios.create({
  timeout: 1000,
  baseURL: import.meta.env.DEV ? 'https://localhost:5002/api' : 'https://lowpressurezone.com/api',
  headers: { 'Access-Control-Allow-Origin': '*' }
})

export const sendGet = async <TResponse>(route: string) => {
  try {
    const response = await axiosInstance.get(route)
    if (response.status === 200) {
      return new ApiResponse<TResponse>(response.status, response.data as TResponse)
    }
    if (response.status === 400) {
      return new ApiResponse<TResponse>(
        response.status,
        undefined,
        response.data as ValidationProblemDetails
      )
    }
    return new ApiResponse<TResponse>(response.status)
  } catch (error) {
    const axiosError = error as AxiosError
    debugger
  }
}

export const sendPut = async <TRequest>(route: string, request: TRequest) => {
  const response = await axiosInstance.put(route, request)
  if (response.status === 204) {
    return new ApiResponse<never>(response.status)
  }
  if (response.status === 400) {
    return new ApiResponse<never>(
      response.status,
      undefined,
      response.data as ValidationProblemDetails
    )
  }
  return new ApiResponse<never>(response.status)
}

export const sendPost = async <TRequest>(route: string, request: TRequest) => {
  const response = await axiosInstance.post(route, request)
  if (response.status === 201) {
    return new ApiResponse<never>(response.status)
  }
  if (response.status === 400) {
    return new ApiResponse<never>(
      response.status,
      undefined,
      response.data as ValidationProblemDetails
    )
  }
  return new ApiResponse<never>(response.status)
}

export const sendDelete = async (route: string) => {
  const response = await axiosInstance.delete(route)
  if (response.status === 204) {
    return new ApiResponse<never>(response.status)
  }
  if (response.status === 400) {
    return new ApiResponse<never>(
      response.status,
      undefined,
      response.data as ValidationProblemDetails
    )
  }
  return new ApiResponse<never>(response.status)
}
