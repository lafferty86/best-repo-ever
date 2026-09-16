# Money Pilot — Architecture & Development Stack

Money Pilot is a fully client-side single-page app: a personal-finance cockpit
(net worth, accounts, transactions, budgets, cash flow, investments, recurring
bills, goals) built with **React 18 + TypeScript on Vite**. All data is
in-memory sample data and fully editable at runtime — there is no backend.

This document describes the complete development stack and how the pieces fit
together.

## Table of contents

1. [Language & type system](#1-language--type-system--typescript)
2. [UI runtime](#2-ui-runtime--react-18)
3. [Build tooling](#3-build-tooling--vite)
4. [State management](#4-state-management--usereducer--context-model-view-update)
5. [Styling](#5-styling--plain-css-with-design-tokens)
6. [Charts](#6-charts--hand-drawn-inline-svg)
7. [Data layer](#7-data-layer)
8. [Domain model & utilities](#8-domain-model--utilities)
9. [Testing / verification](#9-testing--verification--playwright)
10. [Project structure](#10-project-structure)
11. [Build pipeline](#11-build-pipeline)
12. [Dependencies](#12-dependencies)
13. [What's intentionally not in the stack](#13-whats-intentionally-not-in-the-stack)
14. [Prerequisites & distribution](#14-prerequisites--distribution)
15. [Where a backend would slot in](#15-where-a-backend-would-slot-in)

---

## 1. Language & type system — TypeScript

- **TypeScript ^5.6.3**, checked with `tsc`.
- `tsconfig.json` runs in **strict mode** with extra safety flags:
  - `strict: true` — null checks, no implicit `any`, etc.
  - `noUnusedLocals`, `noUnusedParameters` — dead code is a compile error.
  - `noFallthroughCasesInSwitch` — protects the reducer's `switch`.
  - `isolatedModules`, `moduleDetection: "force"` — every file is a real ES module.
- `target: ES2020`; `lib: ["ES2020", "DOM", "DOM.Iterable"]`; `module: "ESNext"`;
  `moduleResolution: "bundler"`; `jsx: "react-jsx"` (automatic JSX runtime — no
  `import React` needed); `noEmit: true` (Vite emits; `tsc` is type-check only).

## 2. UI runtime — React 18

- **react ^18.3.1** + **react-dom ^18.3.1**.
- Mounted in `src/main.tsx` with the React 18 root API:
  `createRoot(document.getElementById("app")!).render(...)`, wrapped in `<StrictMode>`.
- Function components + hooks only (no class components). Hooks used:
  `useReducer`, `useContext`, `createContext`, `useEffect`.
- Type packages: **@types/react ^18.3.12**, **@types/react-dom ^18.3.1**.

## 3. Build tooling — Vite

- **Vite ^5.4.10** is the dev server and bundler.
- **@vitejs/plugin-react ^4.3.4** provides the automatic JSX transform and
  **Fast Refresh** (hot-reload components while preserving state).
- Under the hood Vite uses **esbuild** for dev transforms and **Rollup** for the
  optimized production build.
- `vite.config.ts`:
  - `base: "./"` — relative asset paths, so the build works from any subpath or
    when inlined into a single-file page.
  - `build: { outDir: "dist", emptyOutDir: true }`
  - `server: { port: 5173, host: true }`
- `index.html` is the Vite entry; it loads `/src/main.tsx` as a module and links
  `styles.css`. The favicon is an inline SVG data-URI (no image file).

## 4. State management — `useReducer` + Context (Model-View-Update)

No external state library. The whole store is `src/store.tsx`, structured like
Elm/Redux:

- **`State`** — a single immutable interface holding everything: `page`, `theme`,
  `accounts`, `transactions`, `budgetLimits`, `goals`, `recurrings`, `holdings`,
  filter/search/sort fields, `draft` (add-transaction modal), `toast`,
  `sidebarOpen`, and counters (`nextTxId`, `toastSeq`).
- **`Action`** — a **discriminated union** keyed on a `t` field
  (`{ t: "navigate", page }`, `{ t: "toggleReviewed", id }`, `{ t: "submitDraft" }`,
  `{ t: "setBudget", categoryId, limit }`, …). This makes the reducer `switch`
  exhaustive at compile time.
- **`reducer(state, action)`** — one pure function returning new state. All
  business logic lives here: adding a transaction adjusts the account balance,
  deleting reverses it, budget spend is recomputed from transactions, toasts get
  a fresh id, etc.
- **Selectors** — pure derived helpers next to the reducer: `spentInCategory`,
  `filteredTransactions` (search + category/account filters + sort); plus
  `netWorth` / `assetsTotal` / `liabilitiesTotal` in `components/shared.tsx`.
- **Context wiring** — two contexts (`StateCtx`, `DispatchCtx`), a `StoreProvider`
  calling `useReducer(reducer, undefined, initialState)`, and typed hooks
  `useStore()` / `useDispatch()` that throw if used outside the provider.
- **Side effects** (kept at the edges, since the reducer is pure):
  - **Theme persistence** — written to `localStorage` in the `toggleTheme` action;
    read back in `initialState`.
  - **Toast auto-dismiss** — a `useEffect` in `Toast.tsx` schedules a
    `clearToast`; the toast carries an id so a stale timer can't clear a newer toast.
  - **CSV export** — `downloadTransactionsCsv` builds a CSV string and triggers a
    browser download via a data-URI anchor.

## 5. Styling — plain CSS with design tokens

- A single hand-written **`styles.css`** — no Tailwind, no CSS-in-JS, no preprocessor.
- **Theming via CSS custom properties:** tokens (`--bg`, `--surface`, `--text`,
  `--accent`, `--pos`, `--neg`, `--track`, …) are defined on `:root` and
  overridden under `.theme-dark` / `.theme-light`. Toggling one class on the root
  repaints the whole app; `color-scheme` is set so native `<select>` / date inputs
  follow the theme.
- Layout uses **CSS Grid + Flexbox**; components are addressed by semantic class
  names (`.card`, `.stat-tile`, `.tx-row`, `.budget-row`, …).
- **Responsive** via media queries: at ≤900px the layout collapses to one column
  and the sidebar becomes an off-canvas **drawer** with a click-to-close backdrop;
  a second breakpoint at ≤560px tightens spacing.

## 6. Charts — hand-drawn inline SVG

- `src/charts.tsx` — no charting library. Four React components render raw `<svg>`:
  - **`Donut`** (stroke-dasharray ring), **`AreaLine`** (path + gradient fill),
    **`GroupedBars`**, **`RankedBars`**.
- They take plain props (numbers, colors, labels), compute geometry in JS, and
  animate via CSS transitions — small bundle, fully themeable with the same CSS
  variables.

## 7. Data layer

- **Seed data** in `src/data.ts` — typed arrays of accounts, transactions,
  categories, budgets, holdings, recurrings, goals, and history series, loaded
  once into state at init.
- **In-memory & runtime-editable** — every mutation goes through the reducer;
  there is no database or API.
- **`localStorage`** is the only persistence, and only for the theme
  (`money-pilot-theme`). Reads/writes are wrapped in try/catch so private-mode or
  blocked storage degrades gracefully.

## 8. Domain model & utilities

- `src/types.ts` — domain types (`Account`, `Transaction`, `Budget`, `Holding`,
  `Recurring`, `Goal`, `CashPoint`, …) plus small pure helpers
  (`netContribution`, `holdingGainPct`, `isLiability`, label maps).
- `src/format.ts` — currency/date/percentage formatting (`currency`, `currency0`,
  `currencyCompact`, `shortDate`, `monthYear`, `initials`).

## 9. Testing / verification — Playwright

- **playwright ^1.61.1** (devDependency) drives a headless Chromium in `verify.mjs`.
- The script loads the built app, **walks all 8 pages**, and exercises the key
  interactions — sidebar collapse/drawer, search, add-transaction, budget
  steppers, goal contributions, theme toggle — asserting **zero console errors**.
  It is a smoke test run against `npm run preview`.

```bash
npm run build
npm run preview -- --port 4173 &
CHROME_PATH=/path/to/chrome node verify.mjs   # CHROME_PATH optional
```

## 10. Project structure

```
money-pilot/
├── index.html              # Vite entry; loads /src/main.tsx + styles.css
├── styles.css              # design system (tokens, components, responsive)
├── vite.config.ts          # Vite + React plugin config
├── tsconfig.json           # strict TypeScript config
├── package.json            # deps + scripts
├── verify.mjs              # Playwright smoke test
├── docs/
│   └── ARCHITECTURE.md     # this document
└── src/
    ├── main.tsx            # React root + <StrictMode> + <StoreProvider>
    ├── App.tsx             # shell: Sidebar + Topbar + current page + Modal + Toast + drawer backdrop
    ├── store.tsx           # State, Action, reducer, selectors, Context, hooks, CSV export
    ├── types.ts            # domain types + pure helpers
    ├── data.ts             # seed data
    ├── format.ts           # formatting helpers
    ├── charts.tsx          # SVG chart components
    ├── components/         # Sidebar, Topbar, Modal, Toast, shared primitives (Card, chips, glyph)
    └── pages/              # Dashboard, Accounts, Transactions, Budget,
                            # CashFlow, Investments, Recurring, Goals
```

## 11. Build pipeline

What `npm run build` does:

1. **`tsc -b`** — full type-check across the project; any type or lint-flag error
   fails the build.
2. **`vite build`** — esbuild transpiles, Rollup bundles + tree-shakes + minifies
   into `dist/`: one hashed JS file, one hashed CSS file, and a rewritten
   `index.html`. Output is a **static bundle** (~190 KB JS / ~60 KB gzip).

The dev loop is `npm run dev` → Vite serves modules natively with HMR at
`localhost:5173`.

Scripts (`package.json`):

```bash
npm run dev        # Vite dev server + HMR → localhost:5173
npm run build      # tsc typecheck + vite build → ./dist
npm run preview    # serve the production build
npm run typecheck  # tsc --noEmit
```

## 12. Dependencies

**Runtime (`dependencies`):**

- `react`, `react-dom` — UI framework and DOM renderer.

**Dev (`devDependencies`):**

- `typescript` — type checker.
- `vite` — dev server + production bundler.
- `@vitejs/plugin-react` — JSX transform + Fast Refresh.
- `@types/react`, `@types/react-dom` — type definitions.
- `playwright` — end-to-end smoke test.

That is the entire top-level dependency graph — deliberately tiny.

## 13. What's intentionally not in the stack

- **No backend / database / API**, no auth — a pure client-side SPA with sample data.
- **No state library** (Redux/Zustand/Jotai) — a typed `useReducer` + Context.
- **No data-fetching library** (React Query) — nothing to fetch yet.
- **No router** — navigation is a single `page` field in state.
- **No UI kit** (MUI/Chakra), **no CSS framework**, **no charting library** —
  everything is hand-rolled to stay lean and dependency-light.

## 14. Prerequisites & distribution

- **Only prerequisite:** Node.js 18+ (for npm/Vite). No .NET, no server runtime,
  no native module builds.
- **Distribution:** the `dist/` static bundle deploys to any static host
  (Netlify, Vercel, GitHub Pages, Cloudflare Pages, S3 + CloudFront). The live
  preview is that same bundle inlined into a single-file page.

## 15. Where a backend would slot in

Today data is in-memory. To persist per-user and sync across devices — without
changing the UI much — add a data layer behind the selectors:

- **Fast path:** Supabase or Firebase (Postgres/Firestore + auth).
- **Full control:** a Node/Express or serverless API + Postgres.

Swap the in-memory reads/writes for async calls (adding React Query or similar
for caching). That layer is also where real bank-data aggregation (e.g. Plaid)
and a genuine passcode/auth would live.
