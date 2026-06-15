export default function Pagination({ currentPage, maxPage, onPageChange }) {
  if (maxPage <= 0) return null

  return (
    <div className="pagination">
      <button
        className="pagination-btn"
        disabled={currentPage === 0}
        onClick={() => onPageChange(currentPage - 1)}
      >
        ← Prev
      </button>
      {Array.from({ length: maxPage + 1 }, (_, i) => (
        <button
          key={i}
          className={`pagination-btn${i === currentPage ? ' active' : ''}`}
          onClick={() => onPageChange(i)}
        >
          {i + 1}
        </button>
      ))}
      <button
        className="pagination-btn"
        disabled={currentPage === maxPage}
        onClick={() => onPageChange(currentPage + 1)}
      >
        Next →
      </button>
    </div>
  )
}
