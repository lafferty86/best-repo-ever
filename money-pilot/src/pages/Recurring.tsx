import { useStore } from "../store";
import type { Recurring as RecurringItem } from "../types";
import { cadenceLabel } from "../types";
import * as F from "../format";
import { Card, CardHead } from "../components/shared";

/** Normalize any cadence to an approximate monthly cost for totals. */
const monthlyEquivalent = (r: RecurringItem): number => {
  switch (r.cadence) {
    case "weekly": return (r.amount * 52) / 12;
    case "biweekly": return (r.amount * 26) / 12;
    case "monthly": return r.amount;
    case "yearly": return r.amount / 12;
  }
};

function RecurRow({ r }: { r: RecurringItem; }) {
  return (
    <div className="recur-row">
      <span className="recur-icon" style={{ background: r.color + "22" }}>{r.icon}</span>
      <div className="recur-main">
        <span className="tx-merchant">{r.merchant}</span>
        <span className="tx-sub">{cadenceLabel[r.cadence]} · next {F.shortDate(r.nextDate)}</span>
      </div>
      <span className="recur-cadence">{cadenceLabel[r.cadence]}</span>
      <span className={r.isIncome ? "tx-amt pos big" : "tx-amt big"}>{r.isIncome ? "+" : ""}{F.currency(r.amount)}</span>
    </div>
  );
}

export function Recurring() {
  const { recurrings } = useStore();
  const bills = recurrings.filter((r) => !r.isIncome).sort((a, b) => (a.nextDate < b.nextDate ? -1 : 1));
  const income = recurrings.filter((r) => r.isIncome);
  const monthlyBills = bills.reduce((s, r) => s + monthlyEquivalent(r), 0);
  const monthlyIncome = income.reduce((s, r) => s + monthlyEquivalent(r), 0);
  const subs = bills.filter((r) => r.categoryId === "subs");
  const subsTotal = subs.reduce((s, r) => s + monthlyEquivalent(r), 0);
  const net = monthlyIncome - monthlyBills;

  const stat = (label: string, value: string, cls: string) => (
    <div className="cf-stat">
      <span className="cf-stat-label">{label}</span>
      <span className={`cf-stat-value ${cls}`}>{value}</span>
    </div>
  );

  return (
    <div className="page">
      <div className="cf-stat-row">
        {stat("Recurring income / mo", F.currency0(monthlyIncome), "pos")}
        {stat("Recurring bills / mo", F.currency0(monthlyBills), "neg")}
        {stat("Subscriptions / mo", F.currency0(subsTotal), "")}
        {stat("Net recurring / mo", F.currency0(net), net >= 0 ? "pos" : "neg")}
      </div>
      <div className="grid-2">
        <Card>
          <CardHead title="Bills & subscriptions" right={<span className="muted-sm">{bills.length} active</span>} />
          <div className="recur-list">{bills.map((r) => <RecurRow key={r.id} r={r} />)}</div>
        </Card>
        <div className="stack">
          <Card>
            <CardHead title="Income" />
            <div className="recur-list">{income.map((r) => <RecurRow key={r.id} r={r} />)}</div>
          </Card>
          <Card>
            <CardHead title="Subscription watch" />
            <div className="sub-cloud">
              {subs.map((r) => (
                <span className="sub-chip" key={r.id} style={{ borderColor: r.color + "55" }}>
                  <span>{r.icon}</span>
                  <span>{r.merchant}</span>
                  <span className="sub-price">{F.currency(r.amount)}</span>
                </span>
              ))}
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
