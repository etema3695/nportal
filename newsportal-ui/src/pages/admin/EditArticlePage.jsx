import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { CKEditor } from '@ckeditor/ckeditor5-react'
import ClassicEditor from '@ckeditor/ckeditor5-build-classic'
import { articlesApi } from '../../api/articlesApi'

export default function EditArticlePage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [categories, setCategories] = useState([])
  const [form, setForm] = useState(null)
  const [file, setFile] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    Promise.all([articlesApi.getById(id), articlesApi.getCategories()])
      .then(([article, cats]) => {
        setCategories(cats)
        setForm({
          id: article.id,
          title: article.title ?? '',
          description: article.description ?? '',
          body: article.body ?? '',
          category_Id: article.category_Id ?? '',
          imageTitle: article.imageTitle ?? '',
          imagePath: article.imagePath ?? '',
          isPublished: article.isPublished ?? false
        })
      })
      .catch(() => setError('Failed to load article.'))
  }, [id])

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const fd = new FormData()
      Object.entries(form).forEach(([k, v]) => fd.append(k, v))
      if (file) fd.append('file', file)
      await articlesApi.update(id, fd)
      navigate('/admin/articles')
    } catch {
      setError('Failed to update article.')
    } finally {
      setLoading(false)
    }
  }

  if (!form) return (
    <div className="loading">
      <div className="spinner" />
      {error || 'Loading article…'}
    </div>
  )

  return (
    <div className="container" style={{ maxWidth: '820px' }}>
      <div className="page-header">
        <h1>Edit Article</h1>
        <p>Update your article details and content</p>
      </div>
      {error && <p className="form-error">{error}</p>}
      <form onSubmit={handleSubmit} className="admin-form">
        <div className="form-group">
          <label className="form-label">Title</label>
          <input className="form-input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))}
            placeholder="Article title" required />
        </div>
        <div className="form-group">
          <label className="form-label">Category</label>
          <select className="form-input" value={form.category_Id} onChange={e => setForm(f => ({ ...f, category_Id: e.target.value }))}
            required>
            <option value="">-- Select a category --</option>
            {categories.map(c => <option key={c.id} value={c.id}>{c.code}</option>)}
          </select>
        </div>
        <div className="form-group">
          <label className="form-label">Short Description</label>
          <textarea className="form-input" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
            required rows={3} placeholder="Brief summary" />
        </div>
        <div className="form-group">
          <label className="form-label">Body</label>
          <CKEditor
            editor={ClassicEditor}
            data={form.body}
            onChange={(_, editor) => setForm(f => ({ ...f, body: editor.getData() }))}
          />
        </div>
        <div className="form-group">
          <label className="form-label">Image Title</label>
          <input className="form-input" value={form.imageTitle} onChange={e => setForm(f => ({ ...f, imageTitle: e.target.value }))}
            placeholder="Describe the image" />
        </div>
        {form.imagePath && (
          <div className="form-group">
            <label className="form-label">Current Image</label>
            <img
              src={form.imagePath.replace(/^~\//, '/')}
              alt={form.imageTitle || 'Article image'}
              style={{ maxWidth: '300px', maxHeight: '200px', objectFit: 'cover', borderRadius: 'var(--radius-md)', border: '1px solid var(--color-border)' }}
            />
          </div>
        )}
        <div className="form-group">
          <label className="form-label">Replace Image</label>
          <input className="form-input" type="file" accept="image/*" onChange={e => setFile(e.target.files[0])} />
        </div>
        <div className="form-actions">
          <button type="submit" disabled={loading} className="btn btn-primary" style={{ width: 'auto' }}>
            {loading ? 'Saving…' : 'Save Changes'}
          </button>
          <button type="button" onClick={() => navigate('/admin/articles')} className="btn btn-outline">
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}
