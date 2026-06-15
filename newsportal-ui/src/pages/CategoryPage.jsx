import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { newsportalApi } from '../api/newsportalApi'
import ArticleCard from '../components/ArticleCard'
import Pagination from '../components/Pagination'

export default function CategoryPage() {
  const { id } = useParams()
  const [data, setData] = useState(null)
  const [page, setPage] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    setLoading(true)
    newsportalApi.getCategory(id, page)
      .then(setData)
      .catch(() => setError('Failed to load category.'))
      .finally(() => setLoading(false))
  }, [id, page])

  if (loading) return <p style={{ padding: '2rem' }}>Loading...</p>
  if (error) return <p style={{ padding: '2rem', color: 'red' }}>{error}</p>

  const articles = data?.data?.articles ?? []

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '1.5rem' }}>
      <h2 style={{ marginBottom: '1rem' }}>
        {data?.data?.categories?.find(c => String(c.id) === String(id))?.code ?? 'Category'}
      </h2>
      {articles.length === 0 && <p>No articles in this category yet.</p>}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '1.5rem' }}>
        {articles.map(article => (
          <ArticleCard key={article.id} article={article} />
        ))}
      </div>
      <Pagination currentPage={data?.currentPage ?? 0} maxPage={data?.maxPage ?? 0} onPageChange={setPage} />
    </div>
  )
}
