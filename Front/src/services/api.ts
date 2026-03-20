import type { Category, Person, Transaction } from '../types'

const API_BASE_URL = '/api'

type ProblemDetails = {
  title?: string
  status?: number
  errors?: Record<string, string[]>
}

const buildUrl = (path: string) =>
  `${API_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`

const extractProblemMessage = (data: ProblemDetails | null) => {
  if (!data) return null
  if (data.errors) {
    const firstKey = Object.keys(data.errors)[0]
    const firstMessage = firstKey ? data.errors[firstKey]?.[0] : null
    if (firstMessage) return firstMessage
  }
  return data.title ?? null
}

export const getErrorMessage = (error: unknown) => {
  if (error instanceof Error) return error.message
  if (typeof error === 'string') return error
  return 'Erro inesperado ao comunicar com a API.'
}

const request = async <T>(path: string, options?: RequestInit) => {
  const response = await fetch(buildUrl(path), {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(options?.headers ?? {}),
    },
  })

  if (response.status === 204) return undefined as T

  const data = await response.json().catch(() => null)

  if (!response.ok) {
    const message = extractProblemMessage(data as ProblemDetails | null)
    throw new Error(message ?? response.statusText)
  }

  return data as T
}

export const apiRoutes = {
  people: '/person',
  personById: (id: number) => `/person/${id}`,
  categories: '/category',
  categoryById: (id: number) => `/category/${id}`,
  categoriesByPurpose: (purpose: string) => `/category/purpose/${purpose}`,
  transactions: '/transaction',
  transactionById: (id: number) => `/transaction/${id}`,
  transactionsByPerson: (personId: number) => `/transaction/person/${personId}`,
  transactionsByCategory: (categoryId: number) => `/transaction/category/${categoryId}`,
  availableCategories: (type: string) => `/transaction/available-categories/${type}`,
}

export const getPeople = () => request<Person[]>(apiRoutes.people)
export const createPerson = (payload: Omit<Person, 'id'>) =>
  request<void>(apiRoutes.people, {
    method: 'POST',
    body: JSON.stringify(payload),
  })
export const updatePerson = (id: number, payload: Omit<Person, 'id'>) =>
  request<void>(apiRoutes.personById(id), {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
export const deletePerson = (id: number) =>
  request<void>(apiRoutes.personById(id), { method: 'DELETE' })

export const getCategories = () => request<Category[]>(apiRoutes.categories)
export const createCategory = (payload: Omit<Category, 'id'>) =>
  request<void>(apiRoutes.categories, {
    method: 'POST',
    body: JSON.stringify(payload),
  })
export const updateCategory = (id: number, payload: Omit<Category, 'id'>) =>
  request<void>(apiRoutes.categoryById(id), {
    method: 'PUT',
    body: JSON.stringify(payload),
  })

export const getTransactions = () => request<Transaction[]>(apiRoutes.transactions)
export const createTransaction = (payload: Omit<Transaction, 'id'>) =>
  request<void>(apiRoutes.transactions, {
    method: 'POST',
    body: JSON.stringify(payload),
  })
export const updateTransaction = (id: number, payload: Omit<Transaction, 'id'>) =>
  request<void>(apiRoutes.transactionById(id), {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
export const getAvailableCategories = (type: string) =>
  request<Category[]>(apiRoutes.availableCategories(type))
