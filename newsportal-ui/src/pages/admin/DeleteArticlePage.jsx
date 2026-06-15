import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { articlesApi } from '../../api/articlesApi'

export default function DeleteArticlePage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [article, setArticle] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    articlesApi.getById(id)
      .then(setArticle)
      .catch(() => setError('Article not found.'))
  }, [id])

  async function handleDelete() {
    setLoading(true)
    try {
      await articlesApi.remove(id)
      navigate('/admin/articles')
    } catch {
      setError('Failed to delete article.')
      setLoading(false)
    }
  }

  if (!article) return <p style={{ padding: '2rem' }}>{error || 'Loading…'}</p>

  return (
    <div style={{ maxWidth: '500px', margin: '4rem auto', padding: '2rem', border: '1px solid #ddd', borderRadius: '8px', textAlign: 'center' }}>
      <h2>Delete Article</h2>
      <p>Are you sure you want to delete <strong>{article.title}</strong>?</p>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <div style={{ display: 'flex', justifyContent: 'center', gap: '1rem', marginTop: '1.5rem' }}>
        <button onClick={handleDelete} disabled={loading}
          style={{ padding: '0.6rem 1.2rem', background: 'red', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {loading ? 'Deleting…' : 'Delete'}
        </button>
        <button onClick={() => navigate('/admin/articles')}
          style={{ padding: '0.6rem 1.2rem', borderRadius: '4px', cursor: 'pointer' }}>
          Cancel
        </button>
      </div>
    </div>
  )
}
