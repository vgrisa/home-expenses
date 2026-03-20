export type Person = {
  id: number
  name: string
  age: number
}

export type CategoryPurpose = 0 | 1 | 2

export type Category = {
  id: number
  description: string
  purpose: CategoryPurpose
}

export type TransactionType = 0 | 1

export type Transaction = {
  id: number
  description: string
  value: number
  type: TransactionType
  categoryId: number
  personId: number
}

export type PersonTotals = {
  person: Person
  income: number
  expense: number
  balance: number
}

export type CategoryTotals = {
  category: Category
  income: number
  expense: number
  balance: number
}
