import type { CategoryPurpose, TransactionType } from '../types'

export const nextId = (items: Array<{ id: number }>) =>
  items.length ? Math.max(...items.map((item) => item.id)) + 1 : 1

export const formatMoney = (value: number) => value.toFixed(2)

export const purposeLabel: Record<CategoryPurpose, string> = {
  0: 'Despesa',
  1: 'Receita',
  2: 'Ambas',
}

export const typeLabel: Record<TransactionType, string> = {
  0: 'Despesa',
  1: 'Receita',
}

export const purposeAllowsType = (
  purpose: CategoryPurpose,
  type: TransactionType,
) => purpose === 2 || purpose === type
