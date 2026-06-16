import { useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { useReactTable, getCoreRowModel, flexRender } from '@tanstack/react-table'
import { categoriesApi } from '../../api/categoriesApi'

export default function CategoriesDashboard() {
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    categoriesApi.getAll()
      .then(data => setCategories(data || []))
      .catch(err => {
        if (err?.response?.status === 401 || err?.response?.status === 403) return
        setError('Failed to load categories.')
      })
      .finally(() => setLoading(false))
  }, [])

  async function handleDelete(id) {
    if (!window.confirm('Delete this category?')) return
    try {
      await categoriesApi.remove(id)
      setCategories(prev => prev.filter(c => c.id !== id))
    } catch (err) {
      setError(err?.response?.data?.message ?? 'Failed to delete category.')
    }
  }

  const columns = useMemo(() => [
    { accessorKey: 'code', header: 'Name' },
    { accessorKey: 'parent_Id', header: 'Parent ID', cell: info => info.getValue() ?? '-' },
    { accessorKey: 'createdBy', header: 'Created By' },
    {
      id: 'actions',
      header: 'Actions',
      cell: ({ row }) => (
        <button onClick={() => handleDelete(row.original.id)} className="table-action-btn danger">
          Delete
        </button>
      )
    }
  ], [])

  const table = useReactTable({ data: categories, columns, getCoreRowModel: getCoreRowModel() })

  if (loading) return (
    <div className="loading">
      <div className="spinner" />
      Loading categories…
    </div>
  )

  return (
    <div className="container">
      <div className="dashboard-header">
        <h2 className="dashboard-title">Categories</h2>
        <Link to="/admin/categories/create" className="btn btn-primary" style={{ width: 'auto' }}>
          + New Category
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
      {categories.length === 0 && <p className="empty-state">No categories yet. Create your first one!</p>}
    </div>
  )
}
