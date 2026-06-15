import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { newsportalApi } from '../api/newsportalApi'
import CommentSection from '../components/CommentSection'

export default function ArticlePage() {
  const { id } = useParams()
  const [viewModel, setViewModel] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    setLoading(true)
    newsportalApi.getArticle(id)
      .then(setViewModel)
      .catch(() => setError('Article not found.'))
      .finally(() => setLoading(false))
  }, [id])

  if (loading) return (
    <div className="loading">
      <div className="spinner" />
      Loading article…
    </div>
  )
  if (error) return <p className="form-error" style={{ margin: '2rem auto', maxWidth: '600px' }}>{error}</p>
  if (!viewModel) return null

  const article = viewModel.articles?.[0]
  const comments = viewModel.comments ?? []

  if (!article) return <p className="form-error" style={{ margin: '2rem auto', maxWidth: '600px' }}>Article not found.</p>

  return (
    <div className="article-detail">
      {article.imagePath && (
        <div className="article-hero">
          <img
            src={article.imagePath.replace(/^~\//, '/')}
            alt={article.imageTitle || article.title}
            className="article-hero-img"
          />
        </div>
      )}
      <div className="article-content">
        <div className="article-meta-bar">
          {article.category?.code && (
            <span className="article-badge">{article.category.code}</span>
          )}
          <span className="article-date">
            {article.createdOn ? new Date(article.createdOn).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' }) : ''}
          </span>
          {article.createdBy && <span className="article-author">by {article.createdBy}</span>}
        </div>
        <h1 className="article-title">{article.title}</h1>
        <p className="article-lead">{article.description}</p>
        <div className="article-body" dangerouslySetInnerHTML={{ __html: article.body }} />
      </div>

      <CommentSection articleId={Number(id)} comments={comments} />
    </div>
  )
}
