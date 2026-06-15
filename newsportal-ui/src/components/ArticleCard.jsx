import { Link } from 'react-router-dom'

export default function ArticleCard({ article }) {
  return (
    <div className="card">
      {article.imagePath && (
        <div style={{ overflow: 'hidden' }}>
          <img
            src={article.imagePath.replace(/^~\//, '/')}
            alt={article.imageTitle || article.title}
            className="card-img"
          />
        </div>
      )}
      <div className="card-body">
        <h3 className="card-title">
          <Link to={`/article/${article.id}`}>{article.title}</Link>
        </h3>
        <p className="card-description">{article.description}</p>
        <div className="card-meta">
          {article.category?.code && (
            <span className="card-category">{article.category.code}</span>
          )}
          <span>
            {article.createdOn ? new Date(article.createdOn).toLocaleDateString() : ''}
          </span>
        </div>
      </div>
    </div>
  )
}
