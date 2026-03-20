import { type FormEvent, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  createPerson,
  getErrorMessage,
  getPeople,
  getTransactions,
  updatePerson,
} from '../../services/api'

export function PeopleFormPage() {
  const navigate = useNavigate()
  const { personId } = useParams()
  const isEditing = Boolean(personId)
  const [personName, setPersonName] = useState('')
  const [personAge, setPersonAge] = useState('')
  const [personError, setPersonError] = useState('')
  const [editingPersonId, setEditingPersonId] = useState<number | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const resetForm = () => {
    setEditingPersonId(null)
    setPersonName('')
    setPersonAge('')
    setPersonError('')
  }

  useEffect(() => {
    let isMounted = true

    const loadPerson = async () => {
      setIsLoading(true)
      setPersonError('')
      try {
        if (!personId) {
          resetForm()
          return
        }

        const data = await getPeople()
        const person = data.find((entry) => entry.id === Number(personId))
        if (!person) {
          if (isMounted) navigate('/people')
          return
        }

        if (!isMounted) return
        setEditingPersonId(person.id)
        setPersonName(person.name)
        setPersonAge(String(person.age))
      } catch (error) {
        if (!isMounted) return
        setPersonError(getErrorMessage(error))
      } finally {
        if (isMounted) setIsLoading(false)
      }
    }

    void loadPerson()

    return () => {
      isMounted = false
    }
  }, [personId, navigate])

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    setPersonError('')

    const trimmedName = personName.trim()
    const parsedAge = Number(personAge)

    if (!trimmedName) {
      setPersonError('Informe um nome.')
      return
    }

    if (trimmedName.length > 200) {
      setPersonError('Nome deve ter no máximo 200 caracteres.')
      return
    }

    if (!Number.isFinite(parsedAge) || parsedAge < 0) {
      setPersonError('Informe uma idade válida.')
      return
    }

    if (editingPersonId !== null && parsedAge < 18) {
      try {
        const transactions = await getTransactions()
        const hasIncomeTransactions = transactions.some(
          (transaction) =>
            transaction.personId === editingPersonId && transaction.type === 1,
        )
        if (hasIncomeTransactions) {
          setPersonError('Pessoa menor de idade não pode ter receitas registradas.')
          return
        }
      } catch (error) {
        setPersonError(getErrorMessage(error))
        return
      }
    }

    try {
      if (editingPersonId !== null) {
        await updatePerson(editingPersonId, {
          name: trimmedName,
          age: parsedAge,
        })
      } else {
        await createPerson({ name: trimmedName, age: parsedAge })
      }
      resetForm()
      navigate('/people')
    } catch (error) {
      setPersonError(getErrorMessage(error))
    }
  }

  const handleCancel = () => {
    resetForm()
    navigate('/people')
  }

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>{isEditing ? 'Editar pessoa' : 'Nova pessoa'}</h2>
          <p>Preencha os dados para salvar a pessoa.</p>
        </div>
      </div>

      <div className="panel-grid">
        <form className="card" onSubmit={handleSubmit}>
          <h3>{editingPersonId ? 'Editar pessoa' : 'Nova pessoa'}</h3>
          {isLoading && <p className="muted">Carregando dados da API...</p>}
          <label>
            Nome
            <input
              type="text"
              maxLength={200}
              value={personName}
              onChange={(event) => setPersonName(event.target.value)}
            />
          </label>
          <label>
            Idade
            <input
              type="number"
              min={0}
              value={personAge}
              onChange={(event) => setPersonAge(event.target.value)}
            />
          </label>
          {personError && <p className="error">{personError}</p>}
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
