import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { CKEditor } from '@ckeditor/ckeditor5-react'
import ClassicEditor from '@ckeditor/ckeditor5-build-classic'
import { articlesApi } from '../../api/articlesApi'

export default function CreateArticlePage() {
  const navigate = useNavigate()
  const [categories, setCategories] = useState([])
  const [form, setForm] = useState({ title: '', description: '', body: '', category_Id: '', imageTitle: '' })
  const [file, setFile] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    articlesApi.getCategories().then(setCategories).catch(() => {})
  }, [])

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const fd = new FormData()
      Object.entries(form).forEach(([k, v]) => fd.append(k, v))
      if (file) fd.append('file', file)
      await articlesApi.create(fd)
      navigate('/admin/articles')
    } catch (err) {
      setError('Failed to create article. Please check all fields.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container" style={{ maxWidth: '820px' }}>
      <div className="page-header">
        <h1>Write New Article</h1>
        <p>Create and publish a new story</p>
      </div>
      {error && <p className="form-error">{error}</p>}
      <form onSubmit={handleSubmit} className="admin-form">
        <div className="form-group">
          <label className="form-label">Title</label>
          <input className="form-input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))}
            placeholder="Enter article title" required />
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
            required rows={3} placeholder="Brief summary of the article" />
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
        <div className="form-group">
          <label className="form-label">Image File</label>
          <input className="form-input" type="file" accept="image/*" onChange={e => setFile(e.target.files[0])} />
        </div>
        <div className="form-actions">
          <button type="submit" disabled={loading} className="btn btn-primary" style={{ width: 'auto' }}>
            {loading ? 'Saving…' : 'Save Article'}
          </button>
          <button type="button" onClick={() => navigate('/admin/articles')} className="btn btn-outline">
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}
