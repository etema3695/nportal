import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Navbar() {
  const { user, logout, hasRole } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login')
  }

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-brand">NewsPortal</Link>
      <Link to="/" className="navbar-link">Home</Link>

      {(hasRole('SuperAdmin') || hasRole('Journalist')) && (
        <>
          <Link to="/admin/articles" className="navbar-link">My Articles</Link>
          <Link to="/admin/articles/create" className="navbar-link">✍ Write</Link>
        </>
      )}
      {hasRole('SuperAdmin') && (
        <Link to="/admin/categories" className="navbar-link">Categories</Link>
      )}

      <span className="navbar-right">
        {user ? (
          <>
            <span className="navbar-email">{user.email}</span>
            <button onClick={handleLogout} className="navbar-btn">Log out</button>
          </>
        ) : (
          <>
            <Link to="/login" className="navbar-btn">Log in</Link>
            <Link to="/register" className="navbar-btn navbar-btn-accent">Sign up</Link>
          </>
        )}
      </span>
    </nav>
  )
}
