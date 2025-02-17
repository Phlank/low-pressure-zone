import login from './login/login'
import twoFactor from './two-factor/twoFactor'

const route = (userId?: string) => '/user' + (userId ? `/${userId}` : '')

export default {
  login,
  twoFactor
}
