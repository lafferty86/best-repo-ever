// Seed data — realistic sample so Money Pilot feels alive on first load.
// All state is in-memory and fully editable at runtime.

import type {
  Account, Budget, CashPoint, Category, Goal, Holding, NetWorthPoint,
  Recurring, Transaction,
} from "./types";

export const categories: Category[] = [
  { id: "income", name: "Income", icon: "💰", color: "#22c55e", isIncome: true },
  { id: "groceries", name: "Groceries", icon: "🛒", color: "#f59e0b", isIncome: false },
  { id: "dining", name: "Dining & Bars", icon: "🍽️", color: "#ef4444", isIncome: false },
  { id: "transport", name: "Transport", icon: "🚗", color: "#3b82f6", isIncome: false },
  { id: "shopping", name: "Shopping", icon: "🛍️", color: "#ec4899", isIncome: false },
  { id: "housing", name: "Housing", icon: "🏠", color: "#8b5cf6", isIncome: false },
  { id: "utilities", name: "Utilities", icon: "💡", color: "#06b6d4", isIncome: false },
  { id: "health", name: "Health", icon: "🩺", color: "#14b8a6", isIncome: false },
  { id: "fitness", name: "Fitness", icon: "🏋️", color: "#84cc16", isIncome: false },
  { id: "subs", name: "Subscriptions", icon: "📺", color: "#a855f7", isIncome: false },
  { id: "travel", name: "Travel", icon: "✈️", color: "#0ea5e9", isIncome: false },
  { id: "entertain", name: "Entertainment", icon: "🎬", color: "#f43f5e", isIncome: false },
  { id: "transfer", name: "Transfer", icon: "🔁", color: "#64748b", isIncome: false },
  { id: "misc", name: "Everything Else", icon: "✨", color: "#94a3b8", isIncome: false },
];

const FALLBACK_CATEGORY: Category = {
  id: "misc", name: "Uncategorized", icon: "❓", color: "#94a3b8", isIncome: false,
};

export const categoryById = (id: string): Category =>
  categories.find((c) => c.id === id) ?? { ...FALLBACK_CATEGORY, id };

export const accounts: Account[] = [
  { id: 1, name: "Everyday Checking", institution: "Chase", kind: "checking", balance: 8452.19, change: 320.5, color: "#3b82f6", mask: "4021" },
  { id: 2, name: "High-Yield Savings", institution: "Ally", kind: "savings", balance: 24980.0, change: 104.2, color: "#22c55e", mask: "8830" },
  { id: 3, name: "Sapphire Reserve", institution: "Chase", kind: "credit", balance: -1843.67, change: -412.0, color: "#8b5cf6", mask: "1188" },
  { id: 4, name: "Brokerage", institution: "Fidelity", kind: "investment", balance: 68230.44, change: 1290.8, color: "#f59e0b", mask: "9902" },
  { id: 5, name: "Roth IRA", institution: "Vanguard", kind: "investment", balance: 41120.1, change: 640.3, color: "#14b8a6", mask: "3345" },
  { id: 6, name: "Auto Loan", institution: "Capital One", kind: "loan", balance: -12430.0, change: 380.0, color: "#ef4444", mask: "7761" },
  { id: 7, name: "Travel Rewards", institution: "Amex", kind: "credit", balance: -642.11, change: -128.4, color: "#ec4899", mask: "2204" },
];

export const accountById = (id: number): Account | undefined =>
  accounts.find((a) => a.id === id);

export const accountName = (id: number): string =>
  accountById(id)?.name ?? "Unknown";

/** A month of transactions — deliberately varied for good-looking charts. */
export const transactions: Transaction[] = [
  { id: 1, date: "2026-07-11", merchant: "Whole Foods Market", categoryId: "groceries", accountId: 3, amount: -86.42, note: "", reviewed: true, pending: false },
  { id: 2, date: "2026-07-11", merchant: "Blue Bottle Coffee", categoryId: "dining", accountId: 3, amount: -6.75, note: "", reviewed: false, pending: true },
  { id: 3, date: "2026-07-10", merchant: "Shell", categoryId: "transport", accountId: 1, amount: -52.3, note: "Road trip fuel", reviewed: true, pending: false },
  { id: 4, date: "2026-07-10", merchant: "Netflix", categoryId: "subs", accountId: 3, amount: -22.99, note: "", reviewed: true, pending: false },
  { id: 5, date: "2026-07-09", merchant: "Trader Joe's", categoryId: "groceries", accountId: 1, amount: -63.18, note: "", reviewed: false, pending: false },
  { id: 6, date: "2026-07-09", merchant: "Uber", categoryId: "transport", accountId: 7, amount: -18.4, note: "", reviewed: true, pending: false },
  { id: 7, date: "2026-07-08", merchant: "Sweetgreen", categoryId: "dining", accountId: 3, amount: -15.85, note: "Lunch", reviewed: false, pending: false },
  { id: 8, date: "2026-07-07", merchant: "Acme Corp Payroll", categoryId: "income", accountId: 1, amount: 3120.0, note: "Bi-weekly salary", reviewed: true, pending: false },
  { id: 9, date: "2026-07-07", merchant: "PG&E", categoryId: "utilities", accountId: 1, amount: -142.55, note: "", reviewed: true, pending: false },
  { id: 10, date: "2026-07-06", merchant: "Amazon", categoryId: "shopping", accountId: 3, amount: -74.2, note: "Desk lamp", reviewed: false, pending: false },
  { id: 11, date: "2026-07-06", merchant: "Equinox", categoryId: "fitness", accountId: 1, amount: -215.0, note: "Monthly membership", reviewed: true, pending: false },
  { id: 12, date: "2026-07-05", merchant: "Delta Air Lines", categoryId: "travel", accountId: 7, amount: -388.6, note: "SFO → JFK", reviewed: false, pending: false },
  { id: 13, date: "2026-07-05", merchant: "Spotify", categoryId: "subs", accountId: 3, amount: -11.99, note: "", reviewed: true, pending: false },
  { id: 14, date: "2026-07-04", merchant: "AMC Theatres", categoryId: "entertain", accountId: 3, amount: -34.5, note: "", reviewed: false, pending: false },
  { id: 15, date: "2026-07-03", merchant: "CVS Pharmacy", categoryId: "health", accountId: 1, amount: -28.14, note: "", reviewed: true, pending: false },
  { id: 16, date: "2026-07-03", merchant: "Chipotle", categoryId: "dining", accountId: 3, amount: -13.2, note: "", reviewed: false, pending: false },
  { id: 17, date: "2026-07-02", merchant: "Costco", categoryId: "groceries", accountId: 3, amount: -184.77, note: "Bulk run", reviewed: true, pending: false },
  { id: 18, date: "2026-07-01", merchant: "Oakwood Apartments", categoryId: "housing", accountId: 1, amount: -2450.0, note: "July rent", reviewed: true, pending: false },
  { id: 19, date: "2026-07-01", merchant: "Comcast Xfinity", categoryId: "utilities", accountId: 1, amount: -89.99, note: "Internet", reviewed: true, pending: false },
  { id: 20, date: "2026-06-30", merchant: "Apple", categoryId: "subs", accountId: 3, amount: -2.99, note: "iCloud", reviewed: true, pending: false },
  { id: 21, date: "2026-06-29", merchant: "Lyft", categoryId: "transport", accountId: 7, amount: -22.1, note: "", reviewed: false, pending: false },
  { id: 22, date: "2026-06-28", merchant: "Nike", categoryId: "shopping", accountId: 3, amount: -129.0, note: "Running shoes", reviewed: true, pending: false },
  { id: 23, date: "2026-06-27", merchant: "Tartine Bakery", categoryId: "dining", accountId: 3, amount: -24.6, note: "", reviewed: false, pending: false },
  { id: 24, date: "2026-06-26", merchant: "Interest Payment", categoryId: "income", accountId: 2, amount: 104.22, note: "Savings APY", reviewed: true, pending: false },
  { id: 25, date: "2026-06-25", merchant: "Safeway", categoryId: "groceries", accountId: 1, amount: -71.35, note: "", reviewed: true, pending: false },
  { id: 26, date: "2026-06-24", merchant: "Acme Corp Payroll", categoryId: "income", accountId: 1, amount: 3120.0, note: "Bi-weekly salary", reviewed: true, pending: false },
  { id: 27, date: "2026-06-23", merchant: "Airbnb", categoryId: "travel", accountId: 7, amount: -512.0, note: "Tahoe weekend", reviewed: false, pending: false },
  { id: 28, date: "2026-06-22", merchant: "The Standard Bar", categoryId: "dining", accountId: 3, amount: -58.0, note: "Birthday drinks", reviewed: true, pending: false },
  { id: 29, date: "2026-06-21", merchant: "Best Buy", categoryId: "shopping", accountId: 3, amount: -249.99, note: "Headphones", reviewed: false, pending: false },
  { id: 30, date: "2026-06-20", merchant: "OpenAI", categoryId: "subs", accountId: 3, amount: -20.0, note: "", reviewed: true, pending: false },
  { id: 31, date: "2026-06-19", merchant: "Kaiser Permanente", categoryId: "health", accountId: 1, amount: -45.0, note: "Copay", reviewed: true, pending: false },
  { id: 32, date: "2026-06-18", merchant: "BART", categoryId: "transport", accountId: 1, amount: -9.2, note: "", reviewed: true, pending: false },
  { id: 33, date: "2026-06-17", merchant: "Philz Coffee", categoryId: "dining", accountId: 3, amount: -7.5, note: "", reviewed: false, pending: false },
  { id: 34, date: "2026-06-16", merchant: "Steam", categoryId: "entertain", accountId: 3, amount: -59.99, note: "Summer sale", reviewed: false, pending: false },
  { id: 35, date: "2026-06-15", merchant: "Whole Foods Market", categoryId: "groceries", accountId: 3, amount: -92.11, note: "", reviewed: true, pending: false },
];

export const budgets: Budget[] = [
  { categoryId: "groceries", limit: 700 },
  { categoryId: "dining", limit: 300 },
  { categoryId: "transport", limit: 250 },
  { categoryId: "shopping", limit: 400 },
  { categoryId: "housing", limit: 2450 },
  { categoryId: "utilities", limit: 350 },
  { categoryId: "subs", limit: 120 },
  { categoryId: "fitness", limit: 215 },
  { categoryId: "entertain", limit: 150 },
  { categoryId: "travel", limit: 800 },
];

export const holdings: Holding[] = [
  { symbol: "VTI", name: "Vanguard Total Market", shares: 142, price: 268.4, costBasis: 210.15, color: "#f59e0b" },
  { symbol: "VXUS", name: "Vanguard Intl", shares: 210, price: 62.1, costBasis: 55.8, color: "#3b82f6" },
  { symbol: "AAPL", name: "Apple Inc.", shares: 60, price: 224.9, costBasis: 148.2, color: "#22c55e" },
  { symbol: "NVDA", name: "NVIDIA Corp.", shares: 25, price: 178.5, costBasis: 96.4, color: "#84cc16" },
  { symbol: "MSFT", name: "Microsoft Corp.", shares: 30, price: 465.2, costBasis: 320.1, color: "#14b8a6" },
  { symbol: "BND", name: "Vanguard Total Bond", shares: 180, price: 72.3, costBasis: 74.9, color: "#a855f7" },
];

export const recurrings: Recurring[] = [
  { id: 1, merchant: "Acme Corp Payroll", categoryId: "income", amount: 3120.0, cadence: "biweekly", nextDate: "2026-07-21", icon: "💰", color: "#22c55e", isIncome: true },
  { id: 2, merchant: "Oakwood Apartments", categoryId: "housing", amount: 2450.0, cadence: "monthly", nextDate: "2026-08-01", icon: "🏠", color: "#8b5cf6", isIncome: false },
  { id: 3, merchant: "Equinox", categoryId: "fitness", amount: 215.0, cadence: "monthly", nextDate: "2026-08-06", icon: "🏋️", color: "#84cc16", isIncome: false },
  { id: 4, merchant: "PG&E", categoryId: "utilities", amount: 142.55, cadence: "monthly", nextDate: "2026-08-07", icon: "💡", color: "#06b6d4", isIncome: false },
  { id: 5, merchant: "Comcast Xfinity", categoryId: "utilities", amount: 89.99, cadence: "monthly", nextDate: "2026-08-01", icon: "📶", color: "#0ea5e9", isIncome: false },
  { id: 6, merchant: "Netflix", categoryId: "subs", amount: 22.99, cadence: "monthly", nextDate: "2026-08-10", icon: "🎬", color: "#ef4444", isIncome: false },
  { id: 7, merchant: "Spotify", categoryId: "subs", amount: 11.99, cadence: "monthly", nextDate: "2026-08-05", icon: "🎵", color: "#22c55e", isIncome: false },
  { id: 8, merchant: "iCloud+", categoryId: "subs", amount: 2.99, cadence: "monthly", nextDate: "2026-07-30", icon: "☁️", color: "#64748b", isIncome: false },
  { id: 9, merchant: "Auto Loan", categoryId: "transport", amount: 380.0, cadence: "monthly", nextDate: "2026-08-03", icon: "🚗", color: "#3b82f6", isIncome: false },
];

export const goals: Goal[] = [
  { id: 1, name: "Emergency Fund", icon: "🛡️", color: "#22c55e", target: 30000, saved: 24980, monthly: 800, targetDate: "2026-12" },
  { id: 2, name: "Japan Trip", icon: "🗼", color: "#ef4444", target: 6000, saved: 2150, monthly: 500, targetDate: "2027-03" },
  { id: 3, name: "New MacBook", icon: "💻", color: "#64748b", target: 2500, saved: 1800, monthly: 250, targetDate: "2026-10" },
  { id: 4, name: "House Down Pmt", icon: "🏡", color: "#8b5cf6", target: 80000, saved: 31200, monthly: 1500, targetDate: "2028-06" },
];

export const cashFlowHistory: CashPoint[] = [
  { month: "Feb", income: 6240, expense: 4820 },
  { month: "Mar", income: 6240, expense: 5310 },
  { month: "Apr", income: 6580, expense: 4640 },
  { month: "May", income: 6240, expense: 5120 },
  { month: "Jun", income: 6690, expense: 5480 },
  { month: "Jul", income: 6344, expense: 3980 },
];

export const netWorthHistory: NetWorthPoint[] = [
  { month: "Jan", value: 108420 },
  { month: "Feb", value: 111200 },
  { month: "Mar", value: 115640 },
  { month: "Apr", value: 119880 },
  { month: "May", value: 122310 },
  { month: "Jun", value: 124960 },
  { month: "Jul", value: 127886 },
];
