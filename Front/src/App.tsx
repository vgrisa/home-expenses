import { Navigate, Route, Routes } from 'react-router-dom'
import { Sidebar } from './components/Sidebar'
import { CategoriesFormPage } from './routes/Categories/CategoriesFormPage'
import { CategoriesListPage } from './routes/Categories/CategoriesListPage'
import { PeopleFormPage } from './routes/People/PeopleFormPage'
import { PeopleListPage } from './routes/People/PeopleListPage'
import { ReportsPage } from './routes/Reports/ReportsPage'
import { TransactionsFormPage } from './routes/Transactions/TransactionsFormPage'
import { TransactionsListPage } from './routes/Transactions/TransactionsListPage'

export default function App() {
  return (
    <div className="layout">
      <Sidebar />
      <div className="app">
        <Routes>
          <Route path="/" element={<Navigate to="/people" replace />} />
          <Route path="/people" element={<PeopleListPage />} />
          <Route path="/people/new" element={<PeopleFormPage />} />
          <Route path="/people/:personId/edit" element={<PeopleFormPage />} />
          <Route path="/categories" element={<CategoriesListPage />} />
          <Route path="/categories/new" element={<CategoriesFormPage />} />
          <Route
            path="/categories/:categoryId/edit"
            element={<CategoriesFormPage />}
          />
          <Route path="/transactions" element={<TransactionsListPage />} />
          <Route path="/transactions/new" element={<TransactionsFormPage />} />
          <Route
            path="/transactions/:transactionId/edit"
            element={<TransactionsFormPage />}
          />
          <Route path="/reports" element={<ReportsPage />} />
          <Route path="*" element={<Navigate to="/people" replace />} />
        </Routes>
      </div>
    </div>
  )
}
