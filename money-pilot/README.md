# Money Pilot ✈️💰

**Your financial cockpit** — a fully featured, interactive personal-finance UI built with
**React + TypeScript** and **Vite**.

Money Pilot takes cues from the best personal-finance apps out there — Copilot Money, Monarch Money,
and YNAB — and brings the highlights together in one polished, single-page cockpit.

![Money Pilot dashboard](docs/dashboard.png)

## Features

Eight fully interactive sections, all driven by a single typed state store:

| Section | What it does |
| --- | --- |
| **Dashboard** | Net-worth hero with a computed month-over-month delta and trend chart, asset/liability/income/spend tiles, spending donut, top categories, recent activity, upcoming bills. |
| **Accounts** | Net-worth band (assets vs. liabilities), accounts grouped by type; click any account to jump to its filtered transactions. |
| **Transactions** | Live search across merchant/note/category, filter by category & account, sort by date/amount, "to review" filter, inline re-categorization, mark-reviewed, delete, CSV export, and live inflow/outflow/net totals. |
| **Budget** | Adjustable per-category budgets (spent is computed live from your transactions), donut summary, over-budget warnings, and ± steppers to tune each limit. |
| **Cash Flow** | Income-vs-spending grouped bars, savings-rate stats, net-savings trend, and a monthly breakdown waterfall. |
| **Investments** | Portfolio value hero + allocation donut, holdings with gain/loss, allocation bars, and a cost-basis/return summary. |
| **Recurring** | Bills, subscriptions & paychecks normalized to a monthly figure, with a subscription watch cloud. |
| **Goals** | Savings goals with progress rings and one-tap contributions that update live. |

Plus: **light / dark theme** toggle (persisted to `localStorage`), a **collapsible sidebar**, an
**Add transaction** modal that updates balances in real time, toast notifications, deep-links from
categories/accounts into filtered transactions, and a responsive layout. Every chart (donut,
area/line, grouped bars, ranked bars) is **hand-drawn SVG** — zero charting dependencies.

## Tech stack

- **[React 18](https://react.dev)** + **[TypeScript](https://www.typescriptlang.org)** (strict mode)
- **[Vite](https://vitejs.dev)** — dev server & bundler
- State: a typed `useReducer` + Context store (Model-View-Update pattern — one immutable state
  object, one pure reducer), no external state library.

## Prerequisites

- [Node.js 18+](https://nodejs.org) (provides `npm`)

## Getting started

```bash
cd money-pilot
npm install
npm run dev        # Vite dev server with hot reload → http://localhost:5173
```

### Production build

```bash
npm run build      # tsc typecheck + vite build → ./dist
npm run preview    # serve the built bundle
```

### Type-check only

```bash
npm run typecheck
```

## Project layout

```
money-pilot/
├── index.html
├── styles.css              # design system (light + dark themes)
├── vite.config.ts
├── tsconfig.json
└── src/
    ├── main.tsx            # React entry point
    ├── App.tsx             # shell: sidebar + topbar + current page + modal + toast
    ├── types.ts            # domain model (Account, Transaction, Budget, Goal, …)
    ├── data.ts             # realistic seed data
    ├── format.ts           # currency / date / percentage formatting
    ├── charts.tsx          # hand-drawn SVG charts (Donut, AreaLine, GroupedBars, RankedBars)
    ├── store.tsx           # typed reducer + Context, selectors, CSV export
    ├── components/         # Sidebar, Topbar, Modal, Toast, shared primitives
    └── pages/              # Dashboard, Accounts, Transactions, Budget,
                            # CashFlow, Investments, Recurring, Goals
```

## Verifying it works

`verify.mjs` is a Playwright smoke test that loads the built app, walks every page, exercises the
key interactions (sidebar collapse, search, add transaction, budget steppers, goal contributions,
theme toggle) and asserts there are no console errors.

```bash
npm run build
npm run preview -- --port 4173 &
CHROME_PATH=/path/to/chrome node verify.mjs   # CHROME_PATH optional; omit to use Playwright's browser
```

All data lives in memory and is fully editable at runtime — no backend required.
