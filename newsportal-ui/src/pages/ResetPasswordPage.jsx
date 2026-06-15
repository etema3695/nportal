import { useState } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { accountApi } from '../api/accountApi'

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const code = searchParams.get('code') ?? ''
  const [form, setForm] = useState({ email: '', password: '', confirmPassword: '', code })
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
      await accountApi.resetPassword(form)
      navigate('/login')
    } catch (err) {
      setError(err?.response?.data?.message ?? 'Reset failed.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ maxWidth: '400px', margin: '4rem auto', padding: '2rem', border: '1px solid #ddd', borderRadius: '8px' }}>
      <h2 style={{ marginBottom: '1.5rem' }}>Reset Password</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
        <label>
          Email
          <input type="email" value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))}
            required style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }} />
        </label>
        <label>
          New Password
          <input type="password" value={form.password} onChange={e => setForm(f => ({ ...f, password: e.target.value }))}
            required style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }} />
        </label>
        <label>
          Confirm Password
          <input type="password" value={form.confirmPassword} onChange={e => setForm(f => ({ ...f, confirmPassword: e.target.value }))}
            required style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }} />
        </label>
        <button type="submit" disabled={loading} style={{ padding: '0.6rem' }}>
          {loading ? 'Resetting…' : 'Reset Password'}
        </button>
      </form>
    </div>
  )
}
