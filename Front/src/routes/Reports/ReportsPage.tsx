import { useEffect, useMemo, useState } from 'react'
import { getCategories, getErrorMessage, getPeople, getTransactions } from '../../services/api'
import type { Category, CategoryTotals, Person, PersonTotals, Transaction } from '../../types'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
  minimumFractionDigits: 2,
})

const formatCurrency = (value: number) => currencyFormatter.format(value)

const getBalanceClass = (value: number) => {
  if (value > 0) return 'balance-positive'
  if (value < 0) return 'balance-negative'
  return 'balance-neutral'
}

export function ReportsPage() {
  const [people, setPeople] = useState<Person[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [pageError, setPageError] = useState('')

  useEffect(() => {
    let isMounted = true

    const loadData = async () => {
      setIsLoading(true)
      setPageError('')
      try {
        const [peopleData, categoriesData, transactionsData] = await Promise.all([
          getPeople(),
          getCategories(),
          getTransactions(),
        ])
        if (!isMounted) return
        setPeople(peopleData)
        setCategories(categoriesData)
        setTransactions(transactionsData)
      } catch (error) {
        if (!isMounted) return
        setPageError(getErrorMessage(error))
      } finally {
        if (isMounted) setIsLoading(false)
      }
    }

    void loadData()

    return () => {
      isMounted = false
    }
  }, [])

  const totalsByPerson = useMemo<PersonTotals[]>(() => {
    return people.map((person) => {
      const personTransactions = transactions.filter(
        (transaction) => transaction.personId === person.id,
      )
      const income = personTransactions
        .filter((transaction) => transaction.type === 1)
        .reduce((sum, transaction) => sum + transaction.value, 0)
      const expense = personTransactions
        .filter((transaction) => transaction.type === 0)
        .reduce((sum, transaction) => sum + transaction.value, 0)
      return {
        person,
        income,
        expense,
        balance: income - expense,
      }
    })
  }, [people, transactions])

  const totalsByCategory = useMemo<CategoryTotals[]>(() => {
    return categories.map((category) => {
      const categoryTransactions = transactions.filter(
        (transaction) => transaction.categoryId === category.id,
      )
      const income = categoryTransactions
        .filter((transaction) => transaction.type === 1)
        .reduce((sum, transaction) => sum + transaction.value, 0)
      const expense = categoryTransactions
        .filter((transaction) => transaction.type === 0)
        .reduce((sum, transaction) => sum + transaction.value, 0)
      return {
        category,
        income,
        expense,
        balance: income - expense,
      }
    })
  }, [categories, transactions])

  const totalIncome = transactions
    .filter((transaction) => transaction.type === 1)
    .reduce((sum, transaction) => sum + transaction.value, 0)
  const totalExpense = transactions
    .filter((transaction) => transaction.type === 0)
    .reduce((sum, transaction) => sum + transaction.value, 0)
  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>Totais consolidados</h2>
          <p>Receitas, despesas e saldo por pessoa e categoria.</p>
        </div>
      </div>

      <div className="panel-grid">
        <div className="card">
          <h3>Totais por pessoa</h3>
          {pageError && <p className="error">{pageError}</p>}
          {isLoading && <p className="muted">Carregando dados da API...</p>}
          {totalsByPerson.length === 0 ? (
            <p className="muted">Nenhuma pessoa cadastrada.</p>
          ) : (
            <div className="table report-table">
              <div className="table-row table-head">
                <span>Pessoa</span>
                <span className="table-number">Receitas</span>
                <span className="table-number">Despesas</span>
                <span className="table-number table-nowrap">Saldo</span>
              </div>
              {totalsByPerson.map(({ person, income, expense, balance }) => (
                <div className="table-row" key={person.id}>
                  <span>{person.name}</span>
                  <span className="table-number">{formatCurrency(income)}</span>
                  <span className="table-number">{formatCurrency(expense)}</span>
                  <span
                    className={`table-number table-nowrap ${getBalanceClass(balance)}`}
                  >
                    {formatCurrency(balance)}
                  </span>
                </div>
              ))}
              <div className="table-row table-total">
                <span>Total geral</span>
                <span className="table-number">{formatCurrency(totalIncome)}</span>
                <span className="table-number">{formatCurrency(totalExpense)}</span>
                <span
                  className={`table-number table-nowrap ${getBalanceClass(
                    totalIncome - totalExpense,
                  )}`}
                >
                  {formatCurrency(totalIncome - totalExpense)}
                </span>
              </div>
            </div>
          )}
        </div>

        <div className="card">
          <h3>Totais por categoria</h3>
          {pageError && <p className="error">{pageError}</p>}
          {isLoading && <p className="muted">Carregando dados da API...</p>}
          {totalsByCategory.length === 0 ? (
            <p className="muted">Nenhuma categoria cadastrada.</p>
          ) : (
            <div className="table report-table">
              <div className="table-row table-head">
                <span>Categoria</span>
                <span className="table-number">Receitas</span>
                <span className="table-number">Despesas</span>
                <span className="table-number table-nowrap">Saldo</span>
              </div>
              {totalsByCategory.map(({ category, income, expense, balance }) => (
                <div className="table-row" key={category.id}>
                  <span>{category.description}</span>
                  <span className="table-number">{formatCurrency(income)}</span>
                  <span className="table-number">{formatCurrency(expense)}</span>
                  <span
                    className={`table-number table-nowrap ${getBalanceClass(balance)}`}
                  >
                    {formatCurrency(balance)}
                  </span>
                </div>
              ))}
              <div className="table-row table-total">
                <span>Total geral</span>
                <span className="table-number">{formatCurrency(totalIncome)}</span>
                <span className="table-number">{formatCurrency(totalExpense)}</span>
                <span
                  className={`table-number table-nowrap ${getBalanceClass(
                    totalIncome - totalExpense,
                  )}`}
                >
                  {formatCurrency(totalIncome - totalExpense)}
                </span>
              </div>
            </div>
          )}
        </div>
      </div>
    </section>
  )
}
