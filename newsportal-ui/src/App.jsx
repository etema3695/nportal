import { Routes, Route } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import Navbar from './components/Navbar'
import Footer from './components/Footer'
import ProtectedRoute from './components/ProtectedRoute'

import HomePage from './pages/HomePage'
import ArticlePage from './pages/ArticlePage'
import CategoryPage from './pages/CategoryPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ForgotPasswordPage from './pages/ForgotPasswordPage'
import ResetPasswordPage from './pages/ResetPasswordPage'
import ConfirmEmailPage from './pages/ConfirmEmailPage'

import ArticlesDashboard from './pages/admin/ArticlesDashboard'
import CreateArticlePage from './pages/admin/CreateArticlePage'
import EditArticlePage from './pages/admin/EditArticlePage'
import DeleteArticlePage from './pages/admin/DeleteArticlePage'
import CategoriesDashboard from './pages/admin/CategoriesDashboard'
import CreateCategoryPage from './pages/admin/CreateCategoryPage'

export default function App() {
  return (
    <AuthProvider>
      <div className="app-layout">
        <Navbar />
        <main className="app-main">
          <Routes>
            {/* Public */}
            <Route path="/" element={<HomePage />} />
            <Route path="/article/:id" element={<ArticlePage />} />
            <Route path="/category/:id" element={<CategoryPage />} />

            {/* Auth */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
            <Route path="/confirm-email" element={<ConfirmEmailPage />} />

            {/* Admin — Journalist + SuperAdmin */}
            <Route path="/admin/articles" element={
              <ProtectedRoute roles={['SuperAdmin', 'Journalist']}>
                <ArticlesDashboard />
              </ProtectedRoute>
            } />
            <Route path="/admin/articles/create" element={
              <ProtectedRoute roles={['SuperAdmin', 'Journalist']}>
                <CreateArticlePage />
              </ProtectedRoute>
            } />
            <Route path="/admin/articles/edit/:id" element={
              <ProtectedRoute roles={['SuperAdmin', 'Journalist']}>
                <EditArticlePage />
              </ProtectedRoute>
            } />
            <Route path="/admin/articles/delete/:id" element={
              <ProtectedRoute roles={['SuperAdmin', 'Journalist']}>
                <DeleteArticlePage />
              </ProtectedRoute>
            } />

            {/* Admin — SuperAdmin only */}
            <Route path="/admin/categories" element={
              <ProtectedRoute roles={['SuperAdmin']}>
                <CategoriesDashboard />
              </ProtectedRoute>
            } />
            <Route path="/admin/categories/create" element={
              <ProtectedRoute roles={['SuperAdmin']}>
                <CreateCategoryPage />
              </ProtectedRoute>
            } />

            {/* 404 */}
            <Route path="*" element={
              <div style={{ textAlign: 'center', padding: '4rem' }}>
                <h2>404 — Page not found</h2>
              </div>
            } />
          </Routes>
        </main>
        <Footer />
      </div>
    </AuthProvider>
  )
}
