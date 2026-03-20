import { NavLink } from 'react-router-dom'

const navItems = [
  { to: '/people', label: 'Pessoas', accent: '01' },
  { to: '/categories', label: 'Categorias', accent: '02' },
  { to: '/transactions', label: 'Transações', accent: '03' },
  { to: '/reports', label: 'Totais', accent: '04' },
]

export function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <h1>Home<span>Expense</span></h1>
        <p style={{ fontSize: 11, color: 'var(--text-muted)', marginTop: 2 }}>
          Controle de Gastos
        </p>
      </div>

      <nav className="sidebar-nav">
        {navItems.map(({ to, label, accent }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }: { isActive: boolean }) =>
              `nav-item${isActive ? ' active' : ''}`
            }
          >
            <span className="nav-accent">{accent}</span>
            {label}
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}
