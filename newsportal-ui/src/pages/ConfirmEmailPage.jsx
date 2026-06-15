import { useEffect, useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
import { accountApi } from '../api/accountApi'

export default function ConfirmEmailPage() {
  const [searchParams] = useSearchParams()
  const userId = searchParams.get('userId')
  const code = searchParams.get('code')
  const [message, setMessage] = useState('Confirming your email…')
  const [success, setSuccess] = useState(null)

  useEffect(() => {
    if (!userId || !code) {
      setMessage('Invalid confirmation link.')
      setSuccess(false)
      return
    }
    accountApi.confirmEmail(userId, code)
      .then(data => { setMessage(data.message); setSuccess(true) })
      .catch(err => { setMessage(err?.response?.data?.message ?? 'Confirmation failed.'); setSuccess(false) })
  }, [userId, code])

  return (
    <div style={{ maxWidth: '400px', margin: '4rem auto', padding: '2rem', textAlign: 'center' }}>
      <h2>Email Confirmation</h2>
      <p style={{ color: success === true ? 'green' : success === false ? 'red' : '#555' }}>{message}</p>
      {success && <Link to="/login">Click here to Log in</Link>}
    </div>
  )
}
