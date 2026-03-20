import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getCategories, getErrorMessage, getPeople, getTransactions } from '../../services/api'
import type { Category, Person, Transaction } from '../../types'
import { formatMoney, typeLabel } from '../../utils'

export function TransactionsListPage() {
  const navigate = useNavigate()
  const [people, setPeople] = useState<Person[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [pageError, setPageError] = useState('')

  const loadData = async () => {
    setIsLoading(true)
    setPageError('')
    try {
      const [peopleData, categoriesData, transactionsData] = await Promise.all([
        getPeople(),
        getCategories(),
        getTransactions(),
      ])
      setPeople(peopleData)
      setCategories(categoriesData)
      setTransactions(transactionsData)
    } catch (error) {
      setPageError(getErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadData()
  }, [])

  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>Cadastro de transações</h2>
          <p>Registre receitas e despesas com regras de validação.</p>
        </div>
        <div className="actions">
          <button type="button" onClick={() => navigate('/transactions/new')}>
            Nova transação
          </button>
        </div>
      </div>

      <div className="card">
        <h3>Lista de transações</h3>
        {pageError && <p className="error">{pageError}</p>}
        {isLoading && <p className="muted">Carregando dados da API...</p>}
        {transactions.length === 0 ? (
          <p className="muted">Nenhuma transação registrada.</p>
        ) : (
          <div className="table">
            <div className="table-row table-head">
              <span>ID</span>
              <span>Descrição</span>
              <span>Valor</span>
              <span>Tipo</span>
              <span>Pessoa</span>
              <span>Categoria</span>
              <span className="table-actions-cell">Ações</span>
            </div>
            {transactions.map((transaction) => {
              const person = people.find(
                (personItem) => personItem.id === transaction.personId,
              )
              const category = categories.find(
                (categoryItem) => categoryItem.id === transaction.categoryId,
              )
              return (
                <div className="table-row" key={transaction.id}>
                  <span>{transaction.id}</span>
                  <span>{transaction.description}</span>
                  <span>{formatMoney(transaction.value)}</span>
                  <span>{typeLabel[transaction.type]}</span>
                  <span>{person?.name ?? '-'}</span>
                  <span>{category?.description ?? '-'}</span>
                  <span className="table-actions table-actions-cell">
                    <button
                      type="button"
                      className="icon-button"
                      onClick={() => navigate(`/transactions/${transaction.id}/edit`)}
                      aria-label="Editar transação"
                    >
                      <span aria-hidden="true">✎</span>
                    </button>
                  </span>
                </div>
              )}
            )}
          </div>
        )}
      </div>
    </section>
  )
}
