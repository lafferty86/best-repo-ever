// Core domain model for Money Pilot.

export type Page =
  | "dashboard"
  | "accounts"
  | "transactions"
  | "budget"
  | "cashflow"
  | "investments"
  | "recurring"
  | "goals";

export type Theme = "dark" | "light";

export type AccountKind =
  | "checking"
  | "savings"
  | "credit"
  | "investment"
  | "loan"
  | "cash";

export const accountKindLabel: Record<AccountKind, string> = {
  checking: "Checking",
  savings: "Savings",
  credit: "Credit Card",
  investment: "Investment",
  loan: "Loan",
  cash: "Cash",
};

/** Liabilities count against net worth. */
export const isLiability = (kind: AccountKind): boolean =>
  kind === "credit" || kind === "loan";

export interface Account {
  id: number;
  name: string;
  institution: string;
  kind: AccountKind;
  balance: number;
  /** Change since the previous statement, for the little trend chip. */
  change: number;
  color: string;
  mask: string;
}

/** Signed contribution to net worth. */
export const netContribution = (a: Account): number =>
  isLiability(a.kind) ? -Math.abs(a.balance) : a.balance;

export interface Category {
  id: string;
  name: string;
  icon: string;
  color: string;
  isIncome: boolean;
}

export interface Transaction {
  id: number;
  date: string; // ISO yyyy-MM-dd
  merchant: string;
  categoryId: string;
  accountId: number;
  /** Negative = money out, positive = money in. */
  amount: number;
  note: string;
  reviewed: boolean;
  pending: boolean;
}

export interface Budget {
  categoryId: string;
  limit: number;
}

export interface Holding {
  symbol: string;
  name: string;
  shares: number;
  price: number;
  costBasis: number;
  color: string;
}

export const holdingValue = (h: Holding): number => h.shares * h.price;
export const holdingCost = (h: Holding): number => h.shares * h.costBasis;
export const holdingGain = (h: Holding): number => holdingValue(h) - holdingCost(h);
export const holdingGainPct = (h: Holding): number => {
  const cost = holdingCost(h);
  return cost <= 0 ? 0 : (holdingGain(h) / cost) * 100;
};

export type Cadence = "weekly" | "biweekly" | "monthly" | "yearly";

export const cadenceLabel: Record<Cadence, string> = {
  weekly: "Weekly",
  biweekly: "Bi-weekly",
  monthly: "Monthly",
  yearly: "Yearly",
};

export interface Recurring {
  id: number;
  merchant: string;
  categoryId: string;
  amount: number;
  cadence: Cadence;
  nextDate: string;
  icon: string;
  color: string;
  /** True for income (paycheck), false for a bill/subscription. */
  isIncome: boolean;
}

export interface Goal {
  id: number;
  name: string;
  icon: string;
  color: string;
  target: number;
  saved: number;
  monthly: number;
  targetDate: string; // yyyy-MM
}

export interface CashPoint {
  month: string;
  income: number;
  expense: number;
}

export const cashNet = (p: CashPoint): number => p.income - p.expense;

export interface NetWorthPoint {
  month: string;
  value: number;
}
