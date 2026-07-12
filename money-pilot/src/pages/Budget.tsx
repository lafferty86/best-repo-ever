import { useStore, useDispatch, spentInCategory } from "../store";
import { categoryById } from "../data";
import type { Category } from "../types";
import * as F from "../format";
import { Card, CardHead, CategoryGlyph } from "../components/shared";
import { Donut } from "../charts";

function BudgetRow({ cat, limit }: { cat: Category; limit: number; }) {
  const state = useStore();
  const dispatch = useDispatch();
  const spent = spentInCategory(state.transactions, cat.id);
  const ratio = limit <= 0 ? 0 : spent / limit;
  const over = spent > limit;
  const barClass = over ? "budget-fill over" : ratio > 0.85 ? "budget-fill warn" : "budget-fill";
  return (
    <div className="budget-row">
      <CategoryGlyph cat={cat} size="md" />
      <div className="budget-main">
        <div className="budget-head">
          <span className="budget-name">{cat.name}</span>
          <span className={over ? "budget-amt over" : "budget-amt"}>{F.currency0(spent)} / {F.currency0(limit)}</span>
        </div>
        <div className="budget-track">
          <div className={barClass} style={{ width: `${Math.min(100, ratio * 100)}%`, background: over ? "#ef4444" : cat.color }} />
        </div>
        <span className={over ? "budget-note over" : "budget-note"}>
          {over ? `${F.currency0(spent - limit)} over budget` : `${F.currency0(limit - spent)} left`}
        </span>
      </div>
      <div className="budget-stepper">
        <button className="step-btn" title="Decrease budget" onClick={() => dispatch({ t: "setBudget", categoryId: cat.id, limit: limit - 50 })}>−</button>
        <button className="step-btn" title="Increase budget" onClick={() => dispatch({ t: "setBudget", categoryId: cat.id, limit: limit + 50 })}>＋</button>
      </div>
    </div>
  );
}

export function Budget() {
  const state = useStore();
  const items = Object.entries(state.budgetLimits)
    .map(([id, limit]) => ({ cat: categoryById(id), limit }))
    .sort((a, b) => spentInCategory(state.transactions, b.cat.id) - spentInCategory(state.transactions, a.cat.id));

  const totalBudget = Object.values(state.budgetLimits).reduce((s, v) => s + v, 0);
  const totalSpent = items.reduce((s, x) => s + spentInCategory(state.transactions, x.cat.id), 0);
  const remaining = totalBudget - totalSpent;
  const ratio = totalBudget <= 0 ? 0 : totalSpent / totalBudget;
  const overCount = items.filter((x) => spentInCategory(state.transactions, x.cat.id) > x.limit).length;

  return (
    <div className="page">
      <div className="grid-2">
        <Card className="budget-summary">
          <div className="budget-donut-wrap">
            <Donut size={200} thickness={24} centerLabel={F.percent(ratio * 100)} centerSub="of budget"
              segments={[
                { label: "Spent", value: totalSpent, color: totalSpent > totalBudget ? "#ef4444" : "#6366f1" },
                { label: "Left", value: Math.max(0, remaining), color: "var(--track)" },
              ]} />
            <div className="budget-summary-meta">
              <div className="bsm-row"><span className="muted-sm">Total budget</span><span className="bsm-val">{F.currency0(totalBudget)}</span></div>
              <div className="bsm-row"><span className="muted-sm">Spent</span><span className="bsm-val">{F.currency0(totalSpent)}</span></div>
              <div className="bsm-row"><span className="muted-sm">Remaining</span><span className={remaining >= 0 ? "bsm-val pos" : "bsm-val neg"}>{F.currency0(remaining)}</span></div>
            </div>
          </div>
        </Card>
        <Card>
          <CardHead title="This month" right={overCount > 0
            ? <span className="badge over">{overCount} over</span>
            : <span className="badge ok">On track</span>} />
          <div className="budget-tip">
            <span className="budget-tip-emoji">💡</span>
            <span>{remaining >= 0
              ? `You have ${F.currency0(remaining)} left to spend this month. Use ＋ / − to tune any category.`
              : `You're ${F.currency0(Math.abs(remaining))} over your total plan. Trim a category or raise its limit.`}</span>
          </div>
        </Card>
      </div>
      <Card>
        <CardHead title="Category budgets" right={<span className="muted-sm">Adjustable</span>} />
        <div className="budget-list">{items.map((x) => <BudgetRow key={x.cat.id} cat={x.cat} limit={x.limit} />)}</div>
      </Card>
    </div>
  );
}
