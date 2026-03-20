import { type FormEvent, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import type { CategoryPurpose } from '../../types'
import {
  createCategory,
  getCategories,
  getErrorMessage,
  updateCategory,
} from '../../services/api'

export function CategoriesFormPage() {
  const navigate = useNavigate()
  const { categoryId } = useParams()
  const isEditing = Boolean(categoryId)
  const [categoryDescription, setCategoryDescription] = useState('')
  const [categoryPurpose, setCategoryPurpose] = useState<CategoryPurpose>(0)
  const [categoryError, setCategoryError] = useState('')
  const [editingCategoryId, setEditingCategoryId] = useState<number | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const resetForm = () => {
    setEditingCategoryId(null)
    setCategoryDescription('')
    setCategoryPurpose(0)
    setCategoryError('')
  }

  useEffect(() => {
    let isMounted = true

    const loadCategory = async () => {
      setIsLoading(true)
      setCategoryError('')
      try {
        if (!categoryId) {
          resetForm()
          return
        }

        const data = await getCategories()
        const category = data.find((entry) => entry.id === Number(categoryId))
        if (!category) {
          if (isMounted) navigate('/categories')
          return
        }

        if (!isMounted) return
        setEditingCategoryId(category.id)
        setCategoryDescription(category.description)
        setCategoryPurpose(category.purpose)
      } catch (error) {
        if (!isMounted) return
        setCategoryError(getErrorMessage(error))
      } finally {
        if (isMounted) setIsLoading(false)
      }
    }

    void loadCategory()

    return () => {
      isMounted = false
    }
  }, [categoryId, navigate])

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    setCategoryError('')

    const trimmedDescription = categoryDescription.trim()
    if (!trimmedDescription) {
      setCategoryError('Informe uma descrição.')
      return
    }
    if (trimmedDescription.length > 400) {
      setCategoryError('Descrição deve ter no máximo 400 caracteres.')
      return
    }

    try {
      if (editingCategoryId !== null) {
        await updateCategory(editingCategoryId, {
          description: trimmedDescription,
          purpose: categoryPurpose,
        })
      } else {
        await createCategory({
          description: trimmedDescription,
          purpose: categoryPurpose,
        })
      }
      resetForm()
      navigate('/categories')
    } catch (error) {
      setCategoryError(getErrorMessage(error))
    }
  }

  const handleCancel = () => {
    resetForm()
    navigate('/categories')
  }

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>{isEditing ? 'Editar categoria' : 'Nova categoria'}</h2>
          <p>Defina descrição e finalidade para a categoria.</p>
        </div>
      </div>

      <div className="panel-grid">
        <form className="card" onSubmit={handleSubmit}>
          <h3>{editingCategoryId ? 'Editar categoria' : 'Nova categoria'}</h3>
          {isLoading && <p className="muted">Carregando dados da API...</p>}
          <label>
            Descrição
            <input
              type="text"
              maxLength={400}
              value={categoryDescription}
              onChange={(event) => setCategoryDescription(event.target.value)}
            />
          </label>
          <label>
            Finalidade
            <select
              value={categoryPurpose}
              onChange={(event) =>
                setCategoryPurpose(Number(event.target.value) as CategoryPurpose)
              }
            >
              <option value={0}>Despesa</option>
              <option value={1}>Receita</option>
              <option value={2}>Ambas</option>
            </select>
          </label>
          {categoryError && <p className="error">{categoryError}</p>}
          <div className="actions">
            <button type="submit">Salvar</button>
            <button type="button" className="secondary" onClick={handleCancel}>
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </section>
  )
}
