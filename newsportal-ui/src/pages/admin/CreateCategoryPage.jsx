import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { categoriesApi } from '../../api/categoriesApi'

export default function CreateCategoryPage() {
  const navigate = useNavigate()
  const [all, setAll] = useState([])
  const [form, setForm] = useState({ name: '', parentId: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    categoriesApi.getAll().then(setAll).catch(() => {})
  }, [])

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await categoriesApi.create({ name: form.name, parentId: form.parentId || null })
      navigate('/admin/categories')
    } catch (err) {
      setError(err?.response?.data?.message ?? 'Failed to create category.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="form-card">
      <h2>Create Category</h2>
      <p className="subtitle">Add a new topic for your articles</p>
      {error && <p className="form-error">{error}</p>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label className="form-label">Category Name</label>
          <input className="form-input" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
            placeholder="e.g. Technology, Sports, Health" required />
        </div>
        <div className="form-group">
          <label className="form-label">Parent Category (optional)</label>
          <select className="form-input" value={form.parentId} onChange={e => setForm(f => ({ ...f, parentId: e.target.value }))}>
            <option value="">-- None (top-level) --</option>
            {all.map(c => <option key={c.id} value={c.id}>{c.code}</option>)}
          </select>
        </div>
        <div className="form-actions">
          <button type="submit" disabled={loading} className="btn btn-primary" style={{ width: 'auto' }}>
            {loading ? 'Creating…' : 'Create Category'}
          </button>
          <button type="button" onClick={() => navigate('/admin/categories')} className="btn btn-outline">
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}
