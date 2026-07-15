import { useStore, useDispatch } from "../store";
import type { Goal } from "../types";
import * as F from "../format";
import { Card } from "../components/shared";
import { Donut } from "../charts";

function GoalCard({ g }: { g: Goal; }) {
  const dispatch = useDispatch();
  const pct = g.target <= 0 ? 0 : (g.saved / g.target) * 100;
  const complete = g.saved >= g.target;
  return (
    <div className="goal-card">
      <div className="goal-top">
        <span className="goal-icon" style={{ background: g.color + "22", color: g.color }}>{g.icon}</span>
        <div className="goal-titles">
          <span className="goal-name">{g.name}</span>
          <span className="tx-sub">Target {F.currency0(g.target)} · by {F.monthYear(g.targetDate)}</span>
        </div>
        {complete && <span className="badge ok">Reached 🎉</span>}
      </div>
      <div className="goal-amounts">
        <span className="goal-saved">{F.currency0(g.saved)}</span>
        <span className="goal-of">of {F.currency0(g.target)}</span>
        <span className="goal-pct" style={{ color: g.color }}>{F.percent(pct)}</span>
      </div>
      <div className="goal-track"><div className="goal-fill" style={{ width: `${Math.min(100, pct)}%`, background: g.color }} /></div>
      <div className="goal-foot">
        <span className="tx-sub">{complete ? "Fully funded" : `${F.currency0(g.target - g.saved)} to go · ${F.currency0(g.monthly)}/mo`}</span>
        <div className="goal-btns">
          <button className="mini-btn" disabled={complete} onClick={() => dispatch({ t: "contributeGoal", id: g.id, amount: 100 })}>+ $100</button>
          <button className="mini-btn primary" disabled={complete} onClick={() => dispatch({ t: "addGoalMonthly", id: g.id })}>+ {F.currency0(g.monthly)}</button>
        </div>
      </div>
    </div>
  );
}

export function Goals() {
  const { goals } = useStore();
  const totalTarget = goals.reduce((s, g) => s + g.target, 0);
  const totalSaved = goals.reduce((s, g) => s + g.saved, 0);
  const pct = totalTarget <= 0 ? 0 : (totalSaved / totalTarget) * 100;

  return (
    <div className="page">
      <Card className="goals-summary">
        <div className="goals-summary-inner">
          <Donut size={160} thickness={20} centerLabel={F.percent(pct)} centerSub="funded"
            segments={[
              { label: "Saved", value: totalSaved, color: "#6366f1" },
              { label: "Left", value: Math.max(0, totalTarget - totalSaved), color: "var(--track)" },
            ]} />
          <div className="goals-summary-text">
            <span className="muted-sm">Saved across all goals</span>
            <div className="goals-big">{F.currency0(totalSaved)}</div>
            <span className="tx-sub">of {F.currency0(totalTarget)} total · {F.currency0(totalTarget - totalSaved)} remaining</span>
          </div>
        </div>
      </Card>
      <div className="goal-grid">{goals.map((g) => <GoalCard key={g.id} g={g} />)}</div>
    </div>
  );
}
