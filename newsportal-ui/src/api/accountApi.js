import http from './http'

export const accountApi = {
  me() {
    return http.get('/account/me').then(r => r.data)
  },
  login(payload) {
    return http.post('/account/login', payload).then(r => r.data)
  },
  register(payload) {
    return http.post('/account/register', payload).then(r => r.data)
  },
  logout() {
    return http.post('/account/logout').then(r => r.data)
  },
  confirmEmail(userId, code) {
    return http.get('/account/confirm-email', { params: { userId, code } }).then(r => r.data)
  },
  forgotPassword(email) {
    return http.post('/account/forgot-password', { email }).then(r => r.data)
  },
  resetPassword(payload) {
    return http.post('/account/reset-password', payload).then(r => r.data)
  }
}
