import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '', rememberMe: false })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await login(form.email, form.password, form.rememberMe)
      navigate('/')
    } catch (err) {
      setError(err?.response?.data?.message ?? 'Login failed.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="form-card">
      <h2>Welcome back</h2>
      <p className="subtitle">Sign in to your account to continue</p>
      {error && <p className="form-error">{error}</p>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label className="form-label">Email</label>
          <input className="form-input" type="email" value={form.email}
            onChange={e => setForm(f => ({ ...f, email: e.target.value }))}
            placeholder="you@example.com" required />
        </div>
        <div className="form-group">
          <label className="form-label">Password</label>
          <input className="form-input" type="password" value={form.password}
            onChange={e => setForm(f => ({ ...f, password: e.target.value }))}
            placeholder="••••••••" required />
        </div>
        <div className="form-group">
          <label className="form-check">
            <input type="checkbox" checked={form.rememberMe}
              onChange={e => setForm(f => ({ ...f, rememberMe: e.target.checked }))} />
            Remember me
          </label>
        </div>
        <button type="submit" disabled={loading} className="btn btn-primary">
          {loading ? 'Signing in…' : 'Sign In'}
        </button>
      </form>
      <p className="form-links">
        <Link to="/forgot-password">Forgot password?</Link> &middot; <Link to="/register">Create account</Link>
      </p>
    </div>
  )
}
