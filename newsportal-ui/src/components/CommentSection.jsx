import { useState } from 'react'
import { newsportalApi } from '../api/newsportalApi'
import { useAuth } from '../context/AuthContext'

export default function CommentSection({ articleId, comments = [] }) {
  const { user, hasRole } = useAuth()
  const [list, setList] = useState(comments)
  const [body, setBody] = useState('')
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  // Extract display name from email (part before @)
  const displayName = user?.email?.split('@')[0] ?? ''

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setSubmitting(true)
    try {
      const payload = { name: displayName, body, article_Id: articleId }
      await newsportalApi.addComment(payload)
      setList(prev => [...prev, { ...payload, id: Date.now(), createdOn: new Date().toISOString() }])
      setBody('')
    } catch {
      setError('Failed to post comment. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  async function handleDelete(id) {
    try {
      await newsportalApi.deleteComment(id)
      setList(prev => prev.filter(c => c.id !== id))
    } catch {
      setError('Failed to delete comment.')
    }
  }

  return (
    <section className="comments-section">
      <div className="comments-header">
        <h3 className="comments-title">
          💬 Comments
          <span className="comments-count">{list.length}</span>
        </h3>
      </div>

      {list.length === 0 ? (
        <div className="comments-empty">
          <p>No comments yet. Be the first to share your thoughts!</p>
        </div>
      ) : (
        <div className="comments-list">
          {list.map(c => (
            <div key={c.id} className="comment-card">
              <div className="comment-avatar">
                {(c.name || 'A').charAt(0).toUpperCase()}
              </div>
              <div className="comment-content">
                <div className="comment-meta">
                  <span className="comment-author">{c.name}</span>
                  {c.createdOn && (
                    <span className="comment-time">
                      {new Date(c.createdOn).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                    </span>
                  )}
                </div>
                <p className="comment-body">{c.body}</p>
                {hasRole('SuperAdmin') && (
                  <button onClick={() => handleDelete(c.id)} className="comment-delete">
                    🗑 Remove
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {user ? (
        <div className="comment-form-wrapper">
          <div className="comment-form-header">
            <div className="comment-avatar" style={{ width: 32, height: 32, fontSize: '0.85rem' }}>
              {displayName.charAt(0).toUpperCase()}
            </div>
            <span className="comment-form-user">Commenting as <strong>{displayName}</strong></span>
          </div>
          {error && <p className="form-error">{error}</p>}
          <form onSubmit={handleSubmit} className="comment-form">
            <div className="form-group">
              <textarea
                className="form-input"
                placeholder="Share your thoughts about this article..."
                value={body}
                onChange={e => setBody(e.target.value)}
                required
                rows={4}
              />
            </div>
            <button type="submit" disabled={submitting} className="btn btn-primary" style={{ width: 'auto' }}>
              {submitting ? 'Posting…' : '📤 Post Comment'}
            </button>
          </form>
        </div>
      ) : (
        <div className="comments-login-prompt">
          <p>🔒 <a href="/login">Sign in</a> to leave a comment.</p>
        </div>
      )}
    </section>
  )
}
