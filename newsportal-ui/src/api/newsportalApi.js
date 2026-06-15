import http from './http'

export const newsportalApi = {
  getHome(page = 0) {
    return http.get('/newsportal', { params: { page } }).then(r => r.data)
  },
  getCategory(id, page = 0) {
    return http.get(`/newsportal/category/${id}`, { params: { page } }).then(r => r.data)
  },
  getArticle(id) {
    return http.get(`/newsportal/article/${id}`).then(r => r.data)
  },
  addComment(payload) {
    return http.post('/newsportal/comment', payload).then(r => r.data)
  },
  deleteComment(id) {
    return http.delete(`/newsportal/comment/${id}`).then(r => r.data)
  }
}
