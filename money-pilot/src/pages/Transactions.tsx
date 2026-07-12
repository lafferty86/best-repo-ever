import {
  useStore, useDispatch, filteredTransactions, downloadTransactionsCsv, maskOf,
} from "../store";
import type { Transaction } from "../types";
import { categories, categoryById, accountName } from "../data";
import * as F from "../format";
import { Card, CategoryGlyph } from "../components/shared";

function TxRow({ t }: { t: Transaction; }) {
  const dispatch = useDispatch();
  const cat = categoryById(t.categoryId);
  return (
    <div className={t.reviewed ? "tx-row" : "tx-row unreviewed"}>
      <CategoryGlyph cat={cat} size="md" />
      <div className="tx-row-main">
        <div className="tx-row-top">
          <span className="tx-merchant">{t.merchant}</span>
          {t.pending && <span className="badge pending">Pending</span>}
        </div>
        <span className="tx-sub">{F.shortDate(t.date)} · {accountName(t.accountId)} ••{maskOf(t.accountId)}</span>
        {t.note !== "" && <span className="tx-note">“{t.note}”</span>}
      </div>
      <select className="cat-select" value={t.categoryId}
        onChange={(e) => dispatch({ t: "setTxCategory", id: t.id, categoryId: e.target.value })}>
        {categories.map((c) => <option key={c.id} value={c.id}>{c.icon} {c.name}</option>)}
      </select>
      <span className={t.amount >= 0 ? "tx-amt pos big" : "tx-amt big"}>{F.currency(t.amount)}</span>
      <div className="tx-actions">
        <button className={t.reviewed ? "review-btn done" : "review-btn"}
          title={t.reviewed ? "Reviewed" : "Mark reviewed"}
          onClick={() => dispatch({ t: "toggleReviewed", id: t.id })}>{t.reviewed ? "✓" : "○"}</button>
        <button className="del-btn" title="Delete" onClick={() => dispatch({ t: "deleteTx", id: t.id })}>🗑</button>
      </div>
    </div>
  );
}

export function Transactions() {
  const state = useStore();
  const dispatch = useDispatch();
  const rows = filteredTransactions(state);
  const inflow = rows.filter((t) => t.amount > 0).reduce((s, t) => s + t.amount, 0);
  const outflow = rows.filter((t) => t.amount < 0).reduce((s, t) => s + Math.abs(t.amount), 0);
  const unreviewedCount = state.transactions.filter((t) => !t.reviewed).length;

  return (
    <div className="page">
      <div className="toolbar">
        <div className="search">
          <span className="search-icon">🔍</span>
          <input className="search-input" placeholder="Search merchants, notes, categories…"
            value={state.search} onChange={(e) => dispatch({ t: "setSearch", value: e.target.value })} />
        </div>
        <select className="filter-select" value={state.categoryFilter ?? ""}
          onChange={(e) => dispatch({ t: "setCategoryFilter", value: e.target.value || null })}>
          <option value="">All categories</option>
          {categories.map((c) => <option key={c.id} value={c.id}>{c.icon} {c.name}</option>)}
        </select>
        <select className="filter-select" value={state.accountFilter ?? ""}
          onChange={(e) => dispatch({ t: "setAccountFilter", value: e.target.value ? parseInt(e.target.value, 10) : null })}>
          <option value="">All accounts</option>
          {state.accounts.map((a) => <option key={a.id} value={a.id}>{a.name}</option>)}
        </select>
        <select className="filter-select" value={state.sort}
          onChange={(e) => dispatch({ t: "setSort", value: e.target.value as "date" | "amount" })}>
          <option value="date">Sort: Date</option>
          <option value="amount">Sort: Amount</option>
        </select>
        <button className={state.unreviewedOnly ? "chip-btn active" : "chip-btn"}
          onClick={() => dispatch({ t: "toggleUnreviewedOnly" })}>To review ({unreviewedCount})</button>
        <button className="chip-btn" title="Download the visible transactions as CSV"
          onClick={() => downloadTransactionsCsv(rows)}>⬇ Export CSV</button>
      </div>

      <div className="summary-strip">
        <div className="summary-cell"><span className="summary-label">Showing</span><span className="summary-val">{rows.length} transactions</span></div>
        <div className="summary-cell"><span className="summary-label">Inflow</span><span className="summary-val pos">{F.currency0(inflow)}</span></div>
        <div className="summary-cell"><span className="summary-label">Outflow</span><span className="summary-val neg">{F.currency0(outflow)}</span></div>
        <div className="summary-cell"><span className="summary-label">Net</span><span className={inflow - outflow >= 0 ? "summary-val pos" : "summary-val neg"}>{F.currency0(inflow - outflow)}</span></div>
      </div>

      <Card>
        {rows.length === 0 ? (
          <div className="empty">
            <span className="empty-emoji">🔍</span>
            <span>No transactions match your filters.</span>
          </div>
        ) : (
          <div className="tx-full-list">{rows.map((t) => <TxRow key={t.id} t={t} />)}</div>
        )}
      </Card>
    </div>
  );
}
