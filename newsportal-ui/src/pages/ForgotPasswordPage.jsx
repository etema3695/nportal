import { useState } from 'react'
import { accountApi } from '../api/accountApi'
import { Link } from 'react-router-dom'

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setMessage('')
    setLoading(true)
    try {
      const data = await accountApi.forgotPassword(email)
      setMessage(data.message)
    } catch {
      setError('Something went wrong. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ maxWidth: '400px', margin: '4rem auto', padding: '2rem', border: '1px solid #ddd', borderRadius: '8px' }}>
      <h2 style={{ marginBottom: '1.5rem' }}>Forgot Password</h2>
      {message && <p style={{ color: 'green' }}>{message}</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
        <label>
          Email
          <input type="email" value={email} onChange={e => setEmail(e.target.value)}
            required style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }} />
        </label>
        <button type="submit" disabled={loading} style={{ padding: '0.6rem' }}>
          {loading ? 'Sending…' : 'Send Reset Link'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', fontSize: '0.9rem' }}><Link to="/login">Back to login</Link></p>
    </div>
  )
}
