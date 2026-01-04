import { ApiResponse, type ValidationProblemDetails } from '@/api/apiResponse.ts'
import objectToFormData from '@/utils/objectToFormData.ts'
import type { Ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL

const sendXhrRequest = async <TRequest extends object, TResponse = never>(
  method: 'POST' | 'PUT' | 'GET' | 'DELETE',
  route: string,
  request?: TRequest,
  progressRef?: Ref<number>
): Promise<ApiResponse<TRequest, TResponse>> => {
  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest()
    xhr.open(method, `${API_URL}${route}`, true)
    xhr.withCredentials = true

    xhr.upload.onprogress = (event: ProgressEvent) => {
      if (progressRef === undefined) return
      let progress = 0
      if (event.lengthComputable) progress = event.loaded / event.total
      // console.log(progress)
      progressRef.value = progress
    }

    xhr.onload = () => {
      const parsedResponse = parseXhrResponse<TRequest, TResponse>(xhr)
      const parsedHeaders = parseXhrHeaders(xhr)
      resolve(
        new ApiResponse<TRequest, TResponse>({
          status: xhr.status,
          data: parsedResponse.data,
          validationProblem: parsedResponse.validationProblem,
          error: parsedResponse.error,
          headers: parsedHeaders
        })
      )
    }

    xhr.onerror = (event: ProgressEvent) => {
      resolve(
        new ApiResponse<TRequest, TResponse>({
          status: 0,
          failure: new Error(`XHR request error; event phase: ${event.eventPhase}`)
        })
      )
    }

    xhr.onabort = () => reject(new Error('Request aborted'))

    if (request) xhr.send(objectToFormData(request))
    else xhr.send()
  })
}

const parseXhrResponse = <TRequest extends object, TResponse>(
  xhr: XMLHttpRequest
): {
  data?: TResponse
  validationProblem?: ValidationProblemDetails<TRequest>
  error?: unknown
} => {
  if (!xhr.responseText) return {}
  if (xhr.status >= 200 && xhr.status < 300) {
    return { data: JSON.parse(xhr.responseText) as TResponse }
  }
  if (xhr.status === 400) {
    return { validationProblem: JSON.parse(xhr.responseText) as ValidationProblemDetails<TRequest> }
  }
  return { error: xhr.responseText }
}

const parseXhrHeaders = (xhr: XMLHttpRequest): Headers => {
  return xhr
    .getAllResponseHeaders()
    .split('\r\n')
    .reduce((headers: Headers, currentValue: string) => {
      if (currentValue === '') return headers
      const keyValuePair = currentValue.split(': ')
      if (
        keyValuePair.length !== 2 ||
        keyValuePair[0] === undefined ||
        keyValuePair[1] === undefined
      ) {
        return headers
      }
      headers.append(keyValuePair[0], keyValuePair[1])
      return headers
    }, new Headers())
}

export const sendPostXhr = <TRequest extends object>(
  route: string,
  request: TRequest,
  progress?: Ref<number>
) => sendXhrRequest<TRequest, never>('POST', route, request, progress)

export const sendPutXhr = <TRequest extends object>(
  route: string,
  request: TRequest,
  progress?: Ref<number>
) => sendXhrRequest<TRequest, never>('PUT', route, request, progress)
