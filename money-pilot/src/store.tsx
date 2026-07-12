// Central app state — a typed reducer + context, mirroring an Elm/Redux
// Model-View-Update flow: one immutable state object, one pure reducer.

import {
  createContext, useContext, useReducer, type Dispatch, type ReactNode,
} from "react";
import * as Data from "./data";
import {
  categoryById, accountName, accountById,
} from "./data";
import type {
  Account, Goal, Holding, Page, Recurring, Theme, Transaction,
} from "./types";

export interface TxDraft {
  merchant: string;
  amount: string;
  categoryId: string;
  accountId: number;
  date: string;
  note: string;
  isIncome: boolean;
}

export type Sort = "date" | "amount";
export type ToastKind = "success" | "warning";
export interface Toast { id: number; kind: ToastKind; msg: string; }

export interface State {
  page: Page;
  theme: Theme;
  accounts: Account[];
  transactions: Transaction[];
  budgetLimits: Record<string, number>;
  goals: Goal[];
  recurrings: Recurring[];
  holdings: Holding[];
  search: string;
  categoryFilter: string | null;
  accountFilter: number | null;
  unreviewedOnly: boolean;
  sort: Sort;
  draft: TxDraft | null;
  toast: Toast | null;
  toastSeq: number;
  nextTxId: number;
  sidebarOpen: boolean;
}

const THEME_KEY = "money-pilot-theme";

const storageGet = (key: string): string | null => {
  try { return localStorage.getItem(key); } catch { return null; }
};
const storageSet = (key: string, value: string): void => {
  try { localStorage.setItem(key, value); } catch { /* storage unavailable */ }
};

const wideViewport = (): boolean =>
  typeof window !== "undefined" && window.innerWidth > 900;

export const todayISO = (): string => {
  const d = new Date();
  const p = (n: number) => n.toString().padStart(2, "0");
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
};

export const emptyDraft = (): TxDraft => ({
  merchant: "", amount: "", categoryId: "groceries", accountId: 1,
  date: todayISO(), note: "", isIncome: false,
});

export const initialState = (): State => ({
  page: "dashboard",
  theme: storageGet(THEME_KEY) === "light" ? "light" : "dark",
  accounts: Data.accounts,
  transactions: Data.transactions,
  budgetLimits: Object.fromEntries(Data.budgets.map((b) => [b.categoryId, b.limit])),
  goals: Data.goals,
  recurrings: Data.recurrings,
  holdings: Data.holdings,
  search: "",
  categoryFilter: null,
  accountFilter: null,
  unreviewedOnly: false,
  sort: "date",
  draft: null,
  toast: null,
  toastSeq: 0,
  nextTxId: 1000,
  sidebarOpen: wideViewport(),
});

export type Action =
  | { t: "navigate"; page: Page }
  | { t: "toggleTheme" }
  | { t: "toggleSidebar" }
  | { t: "setSearch"; value: string }
  | { t: "setCategoryFilter"; value: string | null }
  | { t: "setAccountFilter"; value: number | null }
  | { t: "toggleUnreviewedOnly" }
  | { t: "setSort"; value: Sort }
  | { t: "toggleReviewed"; id: number }
  | { t: "setTxCategory"; id: number; categoryId: string }
  | { t: "deleteTx"; id: number }
  | { t: "openModal" }
  | { t: "closeModal" }
  | { t: "updateDraft"; draft: TxDraft }
  | { t: "submitDraft" }
  | { t: "setBudget"; categoryId: string; limit: number }
  | { t: "contributeGoal"; id: number; amount: number }
  | { t: "addGoalMonthly"; id: number }
  | { t: "filterCategory"; categoryId: string }
  | { t: "clearToast"; id: number };

/** Attach a fresh toast to a state patch. */
function withToast(state: State, patch: Partial<State>, kind: ToastKind, msg: string): State {
  const id = state.toastSeq;
  return { ...state, ...patch, toast: { id, kind, msg }, toastSeq: id + 1 };
}

export function reducer(state: State, action: Action): State {
  switch (action.t) {
    case "navigate":
      // On narrow screens the sidebar overlays the page, so close it after navigating.
      return { ...state, page: action.page, sidebarOpen: state.sidebarOpen && wideViewport() };
    case "toggleTheme": {
      const theme: Theme = state.theme === "dark" ? "light" : "dark";
      storageSet(THEME_KEY, theme);
      return { ...state, theme };
    }
    case "toggleSidebar":
      return { ...state, sidebarOpen: !state.sidebarOpen };
    case "setSearch":
      return { ...state, search: action.value };
    case "setCategoryFilter":
      return { ...state, categoryFilter: action.value };
    case "setAccountFilter":
      return { ...state, accountFilter: action.value };
    case "toggleUnreviewedOnly":
      return { ...state, unreviewedOnly: !state.unreviewedOnly };
    case "setSort":
      return { ...state, sort: action.value };
    case "toggleReviewed":
      return {
        ...state,
        transactions: state.transactions.map((t) =>
          t.id === action.id ? { ...t, reviewed: !t.reviewed } : t),
      };
    case "setTxCategory":
      return withToast(state, {
        transactions: state.transactions.map((t) =>
          t.id === action.id ? { ...t, categoryId: action.categoryId } : t),
      }, "success", "Category updated");
    case "deleteTx": {
      // Removing a transaction also reverses its effect on the account balance.
      const tx = state.transactions.find((t) => t.id === action.id);
      const accounts = tx
        ? state.accounts.map((a) =>
            a.id === tx.accountId ? { ...a, balance: a.balance - tx.amount } : a)
        : state.accounts;
      return withToast(state, {
        transactions: state.transactions.filter((t) => t.id !== action.id),
        accounts,
      }, "success", "Transaction removed");
    }
    case "openModal":
      return { ...state, draft: emptyDraft() };
    case "closeModal":
      return { ...state, draft: null };
    case "updateDraft":
      return { ...state, draft: action.draft };
    case "submitDraft": {
      const d = state.draft;
      if (!d) return state;
      const parsed = Math.abs(parseFloat(d.amount));
      if (d.merchant.trim() === "" || !(parsed > 0)) {
        return withToast(state, {}, "warning", "Enter a merchant and a positive amount");
      }
      const signed = d.isIncome ? parsed : -parsed;
      const tx: Transaction = {
        id: state.nextTxId,
        date: d.date,
        merchant: d.merchant.trim(),
        categoryId: d.isIncome ? "income" : d.categoryId,
        accountId: d.accountId,
        amount: signed,
        note: d.note.trim(),
        reviewed: true,
        pending: false,
      };
      return withToast(state, {
        transactions: [tx, ...state.transactions],
        accounts: state.accounts.map((a) =>
          a.id === d.accountId ? { ...a, balance: a.balance + signed } : a),
        nextTxId: state.nextTxId + 1,
        draft: null,
      }, "success", "Transaction added");
    }
    case "setBudget":
      return {
        ...state,
        budgetLimits: { ...state.budgetLimits, [action.categoryId]: Math.max(0, action.limit) },
      };
    case "contributeGoal":
      return withToast(state, {
        goals: state.goals.map((g) =>
          g.id === action.id ? { ...g, saved: Math.min(g.target, g.saved + action.amount) } : g),
      }, "success", `Added ${new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(action.amount)} to goal`);
    case "addGoalMonthly": {
      const goal = state.goals.find((g) => g.id === action.id);
      return withToast(state, {
        goals: state.goals.map((g) =>
          g.id === action.id ? { ...g, saved: Math.min(g.target, g.saved + g.monthly) } : g),
      }, "success", goal ? "Monthly contribution added" : "Contribution added");
    }
    case "filterCategory":
      return { ...state, categoryFilter: action.categoryId, page: "transactions" };
    case "clearToast":
      // Ignore a stale timer trying to clear a newer toast.
      return state.toast && state.toast.id === action.id ? { ...state, toast: null } : state;
  }
}

// ----- Selectors (pure, derived) -----

/** Money spent (positive number) in a category, expenses only. */
export const spentInCategory = (transactions: Transaction[], categoryId: string): number =>
  transactions
    .filter((t) => t.categoryId === categoryId && t.amount < 0)
    .reduce((sum, t) => sum + Math.abs(t.amount), 0);

const matchesSearch = (search: string, t: Transaction): boolean => {
  const q = search.trim().toLowerCase();
  return (
    q === "" ||
    t.merchant.toLowerCase().includes(q) ||
    t.note.toLowerCase().includes(q) ||
    categoryById(t.categoryId).name.toLowerCase().includes(q)
  );
};

/** The transaction list with search box, filters and sort applied. */
export function filteredTransactions(state: State): Transaction[] {
  const rows = state.transactions
    .filter((t) => matchesSearch(state.search, t))
    .filter((t) => state.categoryFilter == null || t.categoryId === state.categoryFilter)
    .filter((t) => state.accountFilter == null || t.accountId === state.accountFilter)
    .filter((t) => !state.unreviewedOnly || !t.reviewed);
  return state.sort === "date"
    ? [...rows].sort((a, b) => (a.date < b.date ? 1 : -1))
    : [...rows].sort((a, b) => Math.abs(b.amount) - Math.abs(a.amount));
}

const csvField = (s: string): string =>
  /[",\n]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s;

/** Build a CSV of the given transactions and trigger a browser download. */
export function downloadTransactionsCsv(rows: Transaction[]): void {
  const line = (t: Transaction) =>
    [
      t.date,
      csvField(t.merchant),
      csvField(categoryById(t.categoryId).name),
      csvField(accountName(t.accountId)),
      t.amount.toFixed(2),
      csvField(t.note),
      t.pending ? "Pending" : t.reviewed ? "Reviewed" : "Unreviewed",
    ].join(",");
  const csv = ["Date,Merchant,Category,Account,Amount,Note,Status", ...rows.map(line)].join("\n");
  const a = document.createElement("a");
  a.href = "data:text/csv;charset=utf-8," + encodeURIComponent(csv);
  a.download = "money-pilot-transactions.csv";
  a.click();
};

export const maskOf = (id: number): string => accountById(id)?.mask ?? "";

// ----- Context wiring -----

const StateCtx = createContext<State | null>(null);
const DispatchCtx = createContext<Dispatch<Action> | null>(null);

export function StoreProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(reducer, undefined, initialState);
  return (
    <StateCtx.Provider value={state}>
      <DispatchCtx.Provider value={dispatch}>{children}</DispatchCtx.Provider>
    </StateCtx.Provider>
  );
}

export function useStore(): State {
  const s = useContext(StateCtx);
  if (!s) throw new Error("useStore must be used within StoreProvider");
  return s;
}

export function useDispatch(): Dispatch<Action> {
  const d = useContext(DispatchCtx);
  if (!d) throw new Error("useDispatch must be used within StoreProvider");
  return d;
}
