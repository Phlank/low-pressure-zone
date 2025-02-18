import { sendGet } from '@/api/fetchFunctions'

const route = '/users/logout'

export default {
  get: () => sendGet<never>(route)
}
