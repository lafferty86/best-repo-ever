import type { ReactNode } from "react";
import { useStore, useDispatch, spentInCategory } from "../store";
import * as Data from "../data";
import { categoryById } from "../data";
import * as F from "../format";
import { AreaLine, Donut, RankedBars } from "../charts";
import {
  Card, CardHead, PctChip, CategoryGlyph,
  netWorth, assetsTotal, liabilitiesTotal,
} from "../components/shared";

function StatTile({ label, value, sub, accent }: { label: string; value: string; sub: ReactNode; accent: string; }) {
  return (
    <div className="stat-tile">
      <div className="stat-accent" style={{ background: accent }} />
      <div>
        <span className="stat-label">{label}</span>
        <div className="stat-value">{value}</div>
        {sub}
      </div>
    </div>
  );
}

export function Dashboard() {
  const state = useStore();
  const dispatch = useDispatch();
  const nw = netWorth(state.accounts);
  const assets = assetsTotal(state.accounts);
  const liabilities = liabilitiesTotal(state.accounts);

  const income = state.transactions.filter((t) => t.amount > 0).reduce((s, t) => s + t.amount, 0);
  const spend = state.transactions.filter((t) => t.amount < 0).reduce((s, t) => s + Math.abs(t.amount), 0);

  // Net-worth month-over-month delta, computed from history.
  const hist = Data.netWorthHistory;
  const nwDelta = hist.length >= 2 ? hist[hist.length - 1].value - hist[hist.length - 2].value : 0;
  const nwPrev = hist.length >= 2 ? hist[hist.length - 2].value : 0;
  const nwPct = nwPrev > 0 ? (nwDelta / nwPrev) * 100 : 0;

  const byCategory = Data.categories
    .filter((c) => !c.isIncome && c.id !== "transfer")
    .map((c) => ({ cat: c, value: spentInCategory(state.transactions, c.id) }))
    .filter((x) => x.value > 0)
    .sort((a, b) => b.value - a.value);

  const donutSegs = byCategory.map((x) => ({ label: x.cat.name, value: x.value, color: x.cat.color }));
  const topCats = byCategory.slice(0, 5).map((x) => ({ icon: x.cat.icon, label: x.cat.name, value: x.value, color: x.cat.color }));

  const recent = [...state.transactions].sort((a, b) => (a.date < b.date ? 1 : -1)).slice(0, 6);
  const upcoming = state.recurrings.filter((r) => !r.isIncome).sort((a, b) => (a.nextDate < b.nextDate ? -1 : 1)).slice(0, 4);

  const jumpToCategory = (name: string) => {
    const cat = Data.categories.find((c) => c.name === name);
    if (cat) dispatch({ t: "filterCategory", categoryId: cat.id });
  };

  return (
    <div className="page">
      <div className="hero">
        <div className="hero-left">
          <span className="hero-label">Total net worth</span>
          <div className="hero-value">{F.currency(nw)}</div>
          <div className="hero-meta">
            <PctChip value={nwPct} />
            <span className="hero-meta-text">＋{F.currency0(nwDelta)} this month</span>
          </div>
        </div>
        <div className="hero-chart">
          <AreaLine w={520} h={120} color="#818cf8" gradId="heroGrad" values={hist.map((p) => p.value)} />
        </div>
      </div>

      <div className="stat-row">
        <StatTile label="Assets" value={F.currency0(assets)} accent="#22c55e"
          sub={<span className="stat-sub up">Cash + investments</span>} />
        <StatTile label="Liabilities" value={F.currency0(liabilities)} accent="#ef4444"
          sub={<span className="stat-sub down">Cards + loans</span>} />
        <StatTile label="Income (mo)" value={F.currency0(income)} accent="#3b82f6"
          sub={<span className="stat-sub up">This period</span>} />
        <StatTile label="Spending (mo)" value={F.currency0(spend)} accent="#f59e0b"
          sub={<span className="stat-sub down">This period</span>} />
      </div>

      <div className="grid-2">
        <Card>
          <CardHead title="Net worth trend" right={<span className="muted-sm">Last 7 months</span>} />
          <AreaLine w={620} h={200} color="#22c55e" gradId="nwGrad" values={hist.map((p) => p.value)} />
          <div className="axis-row">{hist.map((p) => <span key={p.month}>{p.month}</span>)}</div>
        </Card>
        <Card>
          <CardHead title="Spending by category" right={<span className="muted-sm">{F.currency0(spend)}</span>} />
          <div className="donut-wrap">
            <Donut size={180} thickness={22} centerLabel={F.currencyCompact(spend)} centerSub="spent" segments={donutSegs} />
            <div className="legend">
              {byCategory.slice(0, 6).map((x) => (
                <div className="legend-item" key={x.cat.id} style={{ cursor: "pointer" }}
                  onClick={() => dispatch({ t: "filterCategory", categoryId: x.cat.id })}>
                  <span className="dot" style={{ background: x.cat.color }} />
                  <span className="legend-name">{x.cat.name}</span>
                  <span className="legend-val">{F.currency0(x.value)}</span>
                </div>
              ))}
            </div>
          </div>
        </Card>
      </div>

      <div className="grid-2">
        <Card>
          <CardHead title="Recent activity" right={
            <button className="link-btn" onClick={() => dispatch({ t: "navigate", page: "transactions" })}>View all →</button>} />
          <div className="tx-list">
            {recent.map((t) => {
              const cat = categoryById(t.categoryId);
              return (
                <div className="tx-mini" key={t.id}>
                  <CategoryGlyph cat={cat} size="sm" />
                  <div className="tx-mini-main">
                    <span className="tx-merchant">{t.merchant}</span>
                    <span className="tx-sub">{F.shortDate(t.date)} · {cat.name}</span>
                  </div>
                  <span className={t.amount >= 0 ? "tx-amt pos" : "tx-amt"}>{F.currency(t.amount)}</span>
                </div>
              );
            })}
          </div>
        </Card>
        <div className="stack">
          <Card>
            <CardHead title="Top categories" />
            <RankedBars rows={topCats} onClick={jumpToCategory} />
          </Card>
          <Card>
            <CardHead title="Upcoming bills" right={
              <button className="link-btn" onClick={() => dispatch({ t: "navigate", page: "recurring" })}>All →</button>} />
            <div className="bill-list">
              {upcoming.map((r) => (
                <div className="bill-row" key={r.id}>
                  <span className="bill-icon" style={{ background: r.color + "22" }}>{r.icon}</span>
                  <div className="bill-main">
                    <span className="tx-merchant">{r.merchant}</span>
                    <span className="tx-sub">{F.shortDate(r.nextDate)}</span>
                  </div>
                  <span className="tx-amt">{F.currency(r.amount)}</span>
                </div>
              ))}
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
