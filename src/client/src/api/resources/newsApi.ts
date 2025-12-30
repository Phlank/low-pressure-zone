import { sendDelete, sendGet, sendPost } from '@/api/fetchFunctions.ts'

const route = (id?: string) => (id ? `/news/${id}` : '/news')

export default {
  get: () => sendGet<NewsResponse[]>(route()),
  getById: (id: string) => sendGet<NewsResponse>(route(id)),
  post: (request: NewsRequest) => sendPost(route(), request),
  put: (id: string, request: NewsRequest) => sendPost(route(id), request),
  delete: (id: string) => sendDelete(route(id))
}

export interface NewsRequest {
  title: string
  body: string
}

export interface NewsResponse {
  id: string
  title: string
  body: string
  createdAt: string
}
