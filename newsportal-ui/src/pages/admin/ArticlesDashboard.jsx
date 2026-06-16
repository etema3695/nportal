import { useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { useReactTable, getCoreRowModel, flexRender } from '@tanstack/react-table'
import { articlesApi } from '../../api/articlesApi'
import { useAuth } from '../../context/AuthContext'

export default function ArticlesDashboard() {
  const { hasRole } = useAuth()
  const [articles, setArticles] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    articlesApi.getAll()
      .then(data => setArticles(data || []))
      .catch(err => {
        if (err?.response?.status === 401 || err?.response?.status === 403) return
        setError('Failed to load articles.')
      })
      .finally(() => setLoading(false))
  }, [])

  async function handlePublish(id) {
    await articlesApi.publish(id)
    setArticles(prev => prev.map(a => a.id === id ? { ...a, isPublished: true } : a))
  }

  async function handleDelete(id) {
    if (!window.confirm('Delete this article?')) return
    await articlesApi.remove(id)
    setArticles(prev => prev.filter(a => a.id !== id))
  }

  const columns = useMemo(() => [
    { accessorKey: 'title', header: 'Title' },
    { accessorKey: 'category.code', header: 'Category', cell: info => info.getValue() ?? '-' },
    { accessorKey: 'createdBy', header: 'Author' },
    { accessorKey: 'isPublished', header: 'Published', cell: info => info.getValue() ? '✅' : '❌' },
    {
      id: 'actions',
      header: 'Actions',
      cell: ({ row }) => (
        <span className="table-actions">
          <Link to={`/admin/articles/edit/${row.original.id}`} className="table-action-link">Edit</Link>
          {hasRole('SuperAdmin') && !row.original.isPublished && (
            <button onClick={() => handlePublish(row.original.id)} className="table-action-btn success">
              Publish
            </button>
          )}
          <button onClick={() => handleDelete(row.original.id)} className="table-action-btn danger">
            Delete
          </button>
        </span>
      )
    }
  ], [hasRole])

  const table = useReactTable({ data: articles, columns, getCoreRowModel: getCoreRowModel() })

  if (loading) return (
    <div className="loading">
      <div className="spinner" />
      Loading articles…
    </div>
  )

  return (
    <div className="container">
      <div className="dashboard-header">
        <h2 className="dashboard-title">My Articles</h2>
        <Link to="/admin/articles/create" className="btn btn-primary" style={{ width: 'auto' }}>
          + New Article
        </Link>
      </div>
      {error && <p className="form-error">{error}</p>}
      <div className="table-wrapper">
        <table className="data-table">
          <thead>
            {table.getHeaderGroups().map(hg => (
              <tr key={hg.id}>
                {hg.headers.map(h => (
                  <th key={h.id}>
                    {flexRender(h.column.columnDef.header, h.getContext())}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody>
          {table.getRowModel().rows.map(row => (
            <tr key={row.id}>
              {row.getVisibleCells().map(cell => (
                <td key={cell.id}>
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      </div>
      {articles.length === 0 && <p className="empty-state">No articles yet. Create your first one!</p>}
    </div>
  )
}
