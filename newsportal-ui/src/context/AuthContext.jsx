import { createContext, useContext, useState, useEffect } from 'react'
import { accountApi } from '../api/accountApi'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)      // { email, roles: [] }
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    accountApi.me()
      .then(data => setUser(data))
      .catch(() => setUser(null))
      .finally(() => setLoading(false))
  }, [])

  async function login(email, password, rememberMe = false) {
    const data = await accountApi.login({ email, password, rememberMe })
    setUser(data)
    return data
  }

  async function register(email, password, confirmPassword, phone) {
    const data = await accountApi.register({ email, password, confirmPassword, phone })
    setUser(data)
    return data
  }

  async function logout() {
    await accountApi.logout()
    setUser(null)
  }

  function hasRole(role) {
    return user?.roles?.includes(role) ?? false
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, register, hasRole }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  return useContext(AuthContext)
}
