import http from './http'

export const categoriesApi = {
  getAll() {
    return http.get('/categories').then(r => r.data)
  },
  create(payload) {
    return http.post('/categories', payload).then(r => r.data)
  },
  remove(id) {
    return http.delete(`/categories/${id}`).then(r => r.data)
  },
  getArticlesByCategory(id) {
    return http.get(`/categories/${id}/articles`).then(r => r.data)
  }
}
