// Small reusable presentational building blocks.

import type { ReactNode } from "react";
import type { Account, Category, Page } from "../types";
import { isLiability, netContribution } from "../types";
import { currency0, percentSigned } from "../format";

export const netWorth = (accounts: Account[]): number =>
  accounts.reduce((s, a) => s + netContribution(a), 0);

export const assetsTotal = (accounts: Account[]): number =>
  accounts.filter((a) => !isLiability(a.kind)).reduce((s, a) => s + a.balance, 0);

export const liabilitiesTotal = (accounts: Account[]): number =>
  accounts.filter((a) => isLiability(a.kind)).reduce((s, a) => s + Math.abs(a.balance), 0);

export const navItems: [Page, string, string][] = [
  ["dashboard", "🧭", "Dashboard"],
  ["accounts", "🏦", "Accounts"],
  ["transactions", "💳", "Transactions"],
  ["budget", "📊", "Budget"],
  ["cashflow", "🌊", "Cash Flow"],
  ["investments", "📈", "Investments"],
  ["recurring", "🔁", "Recurring"],
  ["goals", "🎯", "Goals"],
];

export const pageTitle: Record<Page, string> = {
  dashboard: "Dashboard", accounts: "Accounts", transactions: "Transactions",
  budget: "Budget", cashflow: "Cash Flow", investments: "Investments",
  recurring: "Recurring", goals: "Goals",
};

export const pageSubtitle: Record<Page, string> = {
  dashboard: "Your money at a glance",
  accounts: "Every account in one cockpit",
  transactions: "Search, categorize and review",
  budget: "Spending against your plan",
  cashflow: "What comes in, what goes out",
  investments: "Portfolio performance & allocation",
  recurring: "Bills, subscriptions & paychecks",
  goals: "Save toward what matters",
};

export function Card({ className = "", children }: { className?: string; children: ReactNode; }) {
  return <div className={`card ${className}`}>{children}</div>;
}

export function CardHead({ title, right }: { title: string; right?: ReactNode; }) {
  return (
    <div className="card-head">
      <h3 className="card-title">{title}</h3>
      {right ?? null}
    </div>
  );
}

export function TrendChip({ value }: { value: number; }) {
  return (
    <span className={`chip ${value >= 0 ? "up" : "down"}`}>
      <span className="chip-arrow">{value >= 0 ? "▲" : "▼"}</span>
      <span>{currency0(Math.abs(value))}</span>
    </span>
  );
}

export function PctChip({ value }: { value: number; }) {
  return (
    <span className={`chip ${value >= 0 ? "up" : "down"}`}>
      <span className="chip-arrow">{value >= 0 ? "▲" : "▼"}</span>
      <span>{percentSigned(value).replace("+", "")}</span>
    </span>
  );
}

export function CategoryGlyph({ cat, size }: { cat: Category; size: "sm" | "md"; }) {
  return (
    <div className={`glyph ${size}`} style={{ background: cat.color + "22", color: cat.color }}>
      <span>{cat.icon}</span>
    </div>
  );
}
