import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { newsportalApi } from '../api/newsportalApi'
import ArticleCard from '../components/ArticleCard'
import Pagination from '../components/Pagination'

export default function HomePage() {
  const [data, setData] = useState(null)
  const [page, setPage] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    setLoading(true)
    newsportalApi.getHome(page)
      .then(setData)
      .catch(() => setError('Failed to load articles.'))
      .finally(() => setLoading(false))
  }, [page])

  if (loading) return (
    <div className="loading">
      <div className="spinner" />
      Loading articles…
    </div>
  )
  if (error) return <p style={{ padding: '2rem', color: 'var(--color-error)' }}>{error}</p>

  const categories = data?.data?.categories ?? []
  const articles = data?.data?.articles ?? []

  return (
    <div className="container">
      <div className="page-header">
        <h1>Latest News</h1>
        <p>Stay informed with the latest articles from our journalists</p>
      </div>

      {/* Category chips */}
      <nav className="chips">
        {categories.map(cat => (
          <Link key={cat.id} to={`/category/${cat.id}`} className="chip">
            {cat.code}
          </Link>
        ))}
      </nav>

      {/* Articles grid */}
      <div className="article-grid">
        {articles.map(article => (
          <ArticleCard key={article.id} article={article} />
        ))}
      </div>

      {articles.length === 0 && (
        <p style={{ textAlign: 'center', color: 'var(--color-text-muted)', padding: '3rem' }}>
          No articles published yet.
        </p>
      )}

      <Pagination currentPage={data?.currentPage ?? 0} maxPage={data?.maxPage ?? 0} onPageChange={setPage} />
    </div>
  )
}
