import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function RegisterPage() {
  const { register } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '', confirmPassword: '', phone: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    if (form.password !== form.confirmPassword) {
      setError('Passwords do not match.')
      return
    }
    setLoading(true)
    try {
      await register(form.email, form.password, form.confirmPassword, form.phone)
      navigate('/')
    } catch (err) {
      const data = err?.response?.data
      if (data && typeof data === 'object') {
        const msgs = Object.values(data).flat().join(' ')
        setError(msgs || 'Registration failed.')
      } else {
        setError('Registration failed.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="form-card">
      <h2>Create account</h2>
      <p className="subtitle">Join NewsPortal to stay informed</p>
      {error && <p className="form-error">{error}</p>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label className="form-label">Email address</label>
          <input className="form-input" type="email" value={form.email}
            onChange={e => setForm(f => ({ ...f, email: e.target.value }))}
            placeholder="you@example.com" required />
        </div>
        <div className="form-group">
          <label className="form-label">Phone number</label>
          <input className="form-input" type="tel" value={form.phone}
            onChange={e => setForm(f => ({ ...f, phone: e.target.value }))}
            placeholder="+1 (555) 000-0000" required />
        </div>
        <div className="form-group">
          <label className="form-label">Password</label>
          <input className="form-input" type="password" value={form.password}
            onChange={e => setForm(f => ({ ...f, password: e.target.value }))}
            placeholder="••••••••" required />
        </div>
        <div className="form-group">
          <label className="form-label">Confirm password</label>
          <input className="form-input" type="password" value={form.confirmPassword}
            onChange={e => setForm(f => ({ ...f, confirmPassword: e.target.value }))}
            placeholder="••••••••" required />
        </div>
        <button type="submit" disabled={loading} className="btn btn-primary">
          {loading ? 'Creating account…' : 'Create Account'}
        </button>
      </form>
      <p className="form-links">
        Already have an account? <Link to="/login">Sign in</Link>
      </p>
    </div>
  )
}
