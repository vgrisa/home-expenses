import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { deletePerson, getErrorMessage, getPeople } from '../../services/api'
import type { Person } from '../../types'

export function PeopleListPage() {
  const navigate = useNavigate()
  const [people, setPeople] = useState<Person[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [pageError, setPageError] = useState('')

  const loadPeople = async () => {
    setIsLoading(true)
    setPageError('')
    try {
      const data = await getPeople()
      setPeople(data)
    } catch (error) {
      setPageError(getErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadPeople()
  }, [])

  const handleDelete = async (personId: number) => {
    setPageError('')
    try {
      await deletePerson(personId)
      await loadPeople()
    } catch (error) {
      setPageError(getErrorMessage(error))
    }
  }

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>Cadastro de pessoas</h2>
          <p>Crie, edite e remova pessoas cadastradas.</p>
        </div>
        <div className="actions">
          <button type="button" onClick={() => navigate('/people/new')}>
            Nova pessoa
          </button>
        </div>
      </div>

      <div className="card">
        <h3>Lista de pessoas</h3>
        {pageError && <p className="error">{pageError}</p>}
        {isLoading && <p className="muted">Carregando dados da API...</p>}
        {people.length === 0 ? (
          <p className="muted">Nenhuma pessoa cadastrada.</p>
        ) : (
          <div className="table">
            <div className="table-row table-head">
              <span>ID</span>
              <span>Nome</span>
              <span>Idade</span>
              <span className="table-actions-cell">Ações</span>
            </div>
            {people.map((person) => (
              <div className="table-row" key={person.id}>
                <span>{person.id}</span>
                <span>{person.name}</span>
                <span>{person.age}</span>
                <span className="table-actions table-actions-cell">
                  <button
                    type="button"
                    className="icon-button"
                    onClick={() => navigate(`/people/${person.id}/edit`)}
                    aria-label="Editar pessoa"
                  >
                    <span aria-hidden="true">✎</span>
                  </button>
                  <button
                    type="button"
                    className="icon-button danger"
                    onClick={() => handleDelete(person.id)}
                    aria-label="Excluir pessoa"
                  >
                    <span aria-hidden="true">✕</span>
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
