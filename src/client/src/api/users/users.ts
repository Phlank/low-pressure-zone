import info from './info/info'
import login from './login/login'
import logout from './logout/logout'
import twoFactor from './two-factor/twoFactor'

const route = (userId?: string) => '/user' + (userId ? `/${userId}` : '')

export default {
  info,
  login,
  logout,
  twoFactor
}
