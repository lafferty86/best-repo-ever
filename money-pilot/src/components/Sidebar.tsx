import { useStore, useDispatch } from "../store";
import { navItems } from "./shared";

export function Sidebar() {
  const { page, sidebarOpen } = useStore();
  const dispatch = useDispatch();
  return (
    <aside className={sidebarOpen ? "sidebar" : "sidebar collapsed"}>
      <div className="brand">
        <div className="brand-mark">
          <svg viewBox="0 0 32 32" width={26} height={26}>
            <rect x={0} y={0} width={32} height={32} rx={8} fill="#6366f1" />
            <path d="M9 21l5-6 4 3 5-8" stroke="white" strokeWidth={2.5} fill="none"
              strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </div>
        <div className="brand-text">
          <span className="brand-name">Money Pilot</span>
          <span className="brand-tag">Financial cockpit</span>
        </div>
      </div>
      <nav className="nav">
        {navItems.map(([p, icon, label]) => (
          <button
            key={p}
            className={page === p ? "nav-item active" : "nav-item"}
            onClick={() => dispatch({ t: "navigate", page: p })}
            title={label}
          >
            <span className="nav-icon">{icon}</span>
            <span className="nav-label">{label}</span>
          </button>
        ))}
      </nav>
      <div className="sidebar-foot">
        <button className="add-btn" onClick={() => dispatch({ t: "openModal" })}>
          <span>＋</span>
          <span className="nav-label">Add transaction</span>
        </button>
      </div>
    </aside>
  );
}
