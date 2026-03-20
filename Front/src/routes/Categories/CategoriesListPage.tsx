import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getCategories, getErrorMessage } from '../../services/api'
import type { Category } from '../../types'
import { purposeLabel } from '../../utils'

export function CategoriesListPage() {
  const navigate = useNavigate()
  const [categories, setCategories] = useState<Category[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [pageError, setPageError] = useState('')

  const loadCategories = async () => {
    setIsLoading(true)
    setPageError('')
    try {
      const data = await getCategories()
      setCategories(data)
    } catch (error) {
      setPageError(getErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadCategories()
  }, [])

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>Cadastro de categorias</h2>
          <p>Crie categorias e defina a finalidade.</p>
        </div>
        <div className="actions">
          <button type="button" onClick={() => navigate('/categories/new')}>
            Nova categoria
          </button>
        </div>
      </div>

      <div className="card">
        <h3>Lista de categorias</h3>
        {pageError && <p className="error">{pageError}</p>}
        {isLoading && <p className="muted">Carregando dados da API...</p>}
        {categories.length === 0 ? (
          <p className="muted">Nenhuma categoria cadastrada.</p>
        ) : (
          <div className="table">
            <div className="table-row table-head">
              <span>ID</span>
              <span>Descrição</span>
              <span>Finalidade</span>
              <span className="table-actions-cell">Ações</span>
            </div>
            {categories.map((category) => (
              <div className="table-row" key={category.id}>
                <span>{category.id}</span>
                <span>{category.description}</span>
                <span
                  className={`purpose-pill purpose-${category.purpose}`}
                  aria-label={`Finalidade: ${purposeLabel[category.purpose]}`}
                >
                  {purposeLabel[category.purpose]}
                </span>
                <span className="table-actions table-actions-cell">
                  <button
                    type="button"
                    className="icon-button"
                    onClick={() => navigate(`/categories/${category.id}/edit`)}
                    aria-label="Editar categoria"
                  >
                    <span aria-hidden="true">✎</span>
                  </button>
                </span>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
