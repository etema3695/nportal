import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Footer() {
  const { user } = useAuth()

  return (
    <footer className="footer">
      <div className="footer-inner">
        <div className="footer-left">
          <span className="footer-brand">NewsPortal</span>
          <span className="footer-text">&copy; {new Date().getFullYear()} All rights reserved.</span>
        </div>
        <nav className="footer-right">
          {!user && (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register">Register</Link>
            </>
          )}
          <Link to="/">Home</Link>
        </nav>
      </div>
    </footer>
  )
}
