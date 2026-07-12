import { useStore } from "./store";
import { Sidebar } from "./components/Sidebar";
import { Topbar } from "./components/Topbar";
import { Modal } from "./components/Modal";
import { Toast } from "./components/Toast";
import { Dashboard } from "./pages/Dashboard";
import { Accounts } from "./pages/Accounts";
import { Transactions } from "./pages/Transactions";
import { Budget } from "./pages/Budget";
import { CashFlow } from "./pages/CashFlow";
import { Investments } from "./pages/Investments";
import { Recurring } from "./pages/Recurring";
import { Goals } from "./pages/Goals";

function CurrentPage() {
  const { page } = useStore();
  switch (page) {
    case "dashboard": return <Dashboard />;
    case "accounts": return <Accounts />;
    case "transactions": return <Transactions />;
    case "budget": return <Budget />;
    case "cashflow": return <CashFlow />;
    case "investments": return <Investments />;
    case "recurring": return <Recurring />;
    case "goals": return <Goals />;
  }
}

export function App() {
  const { theme, sidebarOpen } = useStore();
  return (
    <div className={theme === "dark" ? "app theme-dark" : "app theme-light"} data-sidebar={sidebarOpen ? "open" : "closed"}>
      <Sidebar />
      <div className="main">
        <Topbar />
        <main className="content"><CurrentPage /></main>
      </div>
      <Modal />
      <Toast />
    </div>
  );
}
