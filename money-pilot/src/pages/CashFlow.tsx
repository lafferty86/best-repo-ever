import * as Data from "../data";
import { cashNet } from "../types";
import * as F from "../format";
import { Card, CardHead } from "../components/shared";
import { AreaLine, GroupedBars } from "../charts";

export function CashFlow() {
  const hist = Data.cashFlowHistory;
  const rows = hist.map((p) => [p.month, p.income, p.expense] as [string, number, number]);
  const avgIncome = hist.reduce((s, p) => s + p.income, 0) / hist.length;
  const avgExpense = hist.reduce((s, p) => s + p.expense, 0) / hist.length;
  const avgSaved = avgIncome - avgExpense;
  const savingsRate = avgIncome <= 0 ? 0 : (avgSaved / avgIncome) * 100;
  const current = hist[hist.length - 1];
  const ofIncome = (v: number) => (current.income <= 0 ? 0 : (v / current.income) * 100);

  const stat = (label: string, value: string, cls: string) => (
    <div className="cf-stat">
      <span className="cf-stat-label">{label}</span>
      <span className={`cf-stat-value ${cls}`}>{value}</span>
    </div>
  );

  return (
    <div className="page">
      <div className="cf-stat-row">
        {stat("Avg income / mo", F.currency0(avgIncome), "pos")}
        {stat("Avg spending / mo", F.currency0(avgExpense), "neg")}
        {stat("Avg saved / mo", F.currency0(avgSaved), "pos")}
        {stat("Savings rate", F.percent(savingsRate), "")}
      </div>
      <Card>
        <CardHead title="Income vs. spending" right={
          <div className="legend-inline">
            <span className="li"><span className="dot" style={{ background: "#22c55e" }} /><span>Income</span></span>
            <span className="li"><span className="dot" style={{ background: "#f59e0b" }} /><span>Spending</span></span>
          </div>} />
        <GroupedBars h={220} rows={rows} />
      </Card>
      <div className="grid-2">
        <Card>
          <CardHead title="Net savings trend" right={<span className="muted-sm">6 months</span>} />
          <AreaLine w={560} h={180} color="#22c55e" gradId="netGrad" values={hist.map(cashNet)} />
          <div className="axis-row">{hist.map((p) => <span key={p.month}>{p.month}</span>)}</div>
        </Card>
        <Card>
          <CardHead title={`${current.month} breakdown`} />
          <div className="waterfall">
            <div className="wf-row">
              <span className="wf-label">Income</span>
              <div className="wf-bar"><div className="wf-fill pos" style={{ width: "100%" }} /></div>
              <span className="wf-val pos">{F.currency0(current.income)}</span>
            </div>
            <div className="wf-row">
              <span className="wf-label">Spending</span>
              <div className="wf-bar"><div className="wf-fill neg" style={{ width: `${ofIncome(current.expense)}%` }} /></div>
              <span className="wf-val neg">{F.currency0(current.expense)}</span>
            </div>
            <div className="wf-divider" />
            <div className="wf-row">
              <span className="wf-label strong">Net saved</span>
              <div className="wf-bar"><div className="wf-fill accent" style={{ width: `${ofIncome(cashNet(current))}%` }} /></div>
              <span className="wf-val strong">{F.currency0(cashNet(current))}</span>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
