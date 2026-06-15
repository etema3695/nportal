import http from './http'

export const articlesApi = {
  getAll() {
    return http.get('/articles').then(r => r.data)
  },
  getById(id) {
    return http.get(`/articles/${id}`).then(r => r.data)
  },
  getCategories() {
    return http.get('/articles/categories').then(r => r.data)
  },
  create(formData) {
    return http.post('/articles', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    }).then(r => r.data)
  },
  update(id, formData) {
    return http.put(`/articles/${id}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    }).then(r => r.data)
  },
  remove(id) {
    return http.delete(`/articles/${id}`).then(r => r.data)
  },
  publish(id) {
    return http.post(`/articles/${id}/publish`).then(r => r.data)
  }
}
