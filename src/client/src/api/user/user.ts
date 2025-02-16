import login from './login/login'

const route = (userId?: string) => '/user' + (userId ? `/${userId}` : '')

export default {
  login
}
