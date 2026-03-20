import { type FormEvent, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  createTransaction,
  getCategories,
  getErrorMessage,
  getPeople,
  getTransactions,
  updateTransaction,
} from '../../services/api'
import type { Category, Person, TransactionType } from '../../types'
import { purposeAllowsType } from '../../utils'

export function TransactionsFormPage() {
  const navigate = useNavigate()
  const { transactionId } = useParams()
  const isEditing = Boolean(transactionId)
  const [people, setPeople] = useState<Person[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [transactionDescription, setTransactionDescription] = useState('')
  const [transactionValue, setTransactionValue] = useState('')
  const [transactionType, setTransactionType] = useState<TransactionType>(0)
  const [transactionCategoryId, setTransactionCategoryId] = useState('')
  const [transactionPersonId, setTransactionPersonId] = useState('')
  const [transactionError, setTransactionError] = useState('')
  const [editingTransactionId, setEditingTransactionId] =
    useState<number | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const resetForm = () => {
    setEditingTransactionId(null)
    setTransactionDescription('')
    setTransactionValue('')
    setTransactionType(0)
    setTransactionCategoryId('')
    setTransactionPersonId('')
    setTransactionError('')
  }

  useEffect(() => {
    let isMounted = true

    const loadData = async () => {
      setIsLoading(true)
      setTransactionError('')
      try {
        const [peopleData, categoriesData, transactionsData] = await Promise.all([
          getPeople(),
          getCategories(),
          getTransactions(),
        ])

        if (!isMounted) return
        setPeople(peopleData)
        setCategories(categoriesData)
        if (!transactionId) {
          resetForm()
          return
        }

        const transaction = transactionsData.find(
          (entry) => entry.id === Number(transactionId),
        )
        if (!transaction) {
          navigate('/transactions')
          return
        }

        setEditingTransactionId(transaction.id)
        setTransactionDescription(transaction.description)
        setTransactionValue(String(transaction.value))
        setTransactionType(transaction.type)
        setTransactionCategoryId(String(transaction.categoryId))
        setTransactionPersonId(String(transaction.personId))
      } catch (error) {
        if (!isMounted) return
        setTransactionError(getErrorMessage(error))
      } finally {
        if (isMounted) setIsLoading(false)
      }
    }

    void loadData()

    return () => {
      isMounted = false
    }
  }, [transactionId, navigate])

  const selectedPerson = useMemo(
    () => people.find((person) => String(person.id) === transactionPersonId),
    [people, transactionPersonId],
  )

  const isMinor = selectedPerson ? selectedPerson.age < 18 : false

  useEffect(() => {
    if (isMinor && transactionType === 1) {
      setTransactionType(0)
      setTransactionError('Pessoa menor de idade só pode registrar despesas.')
    }
  }, [isMinor, transactionType])

  const availableCategories = useMemo(() => {
    return categories.filter((category) =>
      purposeAllowsType(category.purpose, transactionType),
    )
  }, [categories, transactionType])

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    setTransactionError('')

    const trimmedDescription = transactionDescription.trim()
    const parsedValue = Number(transactionValue)
    const selectedCategory = categories.find(
      (category) => String(category.id) === transactionCategoryId,
    )

    if (!trimmedDescription) {
      setTransactionError('Informe uma descrição.')
      return
    }
    if (trimmedDescription.length > 400) {
      setTransactionError('Descrição deve ter no máximo 400 caracteres.')
      return
    }
    if (!Number.isFinite(parsedValue) || parsedValue <= 0) {
      setTransactionError('Informe um valor positivo.')
      return
    }
    if (!transactionPersonId) {
      setTransactionError('Selecione uma pessoa.')
      return
    }
    if (!selectedCategory) {
      setTransactionError('Selecione uma categoria.')
      return
    }
    if (isMinor && transactionType === 1) {
      setTransactionError('Pessoa menor de idade só pode registrar despesas.')
      return
    }
    if (!purposeAllowsType(selectedCategory.purpose, transactionType)) {
      setTransactionError('Categoria não permite este tipo de transação.')
      return
    }

    try {
      const payload = {
        description: trimmedDescription,
        value: parsedValue,
        type: transactionType,
        categoryId: selectedCategory.id,
        personId: Number(transactionPersonId),
      }

      if (editingTransactionId !== null) {
        await updateTransaction(editingTransactionId, payload)
      } else {
        await createTransaction(payload)
      }
      resetForm()
      navigate('/transactions')
    } catch (error) {
      setTransactionError(getErrorMessage(error))
    }
  }

  const handleCancel = () => {
    resetForm()
    navigate('/transactions')
  }

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>{isEditing ? 'Editar transação' : 'Nova transação'}</h2>
          <p>Registre receitas e despesas com regras de validação.</p>
        </div>
      </div>

      <div className="panel-grid">
        <form className="card" onSubmit={handleSubmit}>
          <h3>{editingTransactionId ? 'Editar transação' : 'Nova transação'}</h3>
          {isLoading && <p className="muted">Carregando dados da API...</p>}
          <label>
            Descrição
            <input
              type="text"
              maxLength={400}
              value={transactionDescription}
              onChange={(event) => setTransactionDescription(event.target.value)}
            />
          </label>
          <label>
            Valor
            <input
              type="number"
              min={0}
              step="0.01"
              value={transactionValue}
              onChange={(event) => setTransactionValue(event.target.value)}
            />
          </label>
          <label>
            Pessoa
            <select
              value={transactionPersonId}
              onChange={(event) => setTransactionPersonId(event.target.value)}
            >
              <option value="">Selecione</option>
              {people.map((person) => (
                <option key={person.id} value={person.id}>
                  {person.name}
                </option>
              ))}
            </select>
          </label>
          <label>
            Tipo
            <select
              value={transactionType}
              onChange={(event) =>
                setTransactionType(Number(event.target.value) as TransactionType)
              }
              disabled={isMinor}
            >
              <option value={0}>Despesa</option>
              <option value={1}>Receita</option>
            </select>
          </label>
          <label>
            Categoria
            <select
              value={transactionCategoryId}
              onChange={(event) => setTransactionCategoryId(event.target.value)}
            >
              <option value="">Selecione</option>
              {availableCategories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.description}
                </option>
              ))}
            </select>
          </label>
          {isMinor && (
            <p className="hint">Pessoa menor de idade só pode registrar despesa.</p>
          )}
          {transactionError && <p className="error">{transactionError}</p>}
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
