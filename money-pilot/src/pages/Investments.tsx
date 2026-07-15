import { useStore } from "../store";
import { holdingValue, holdingCost, holdingGainPct } from "../types";
import * as F from "../format";
import { Card, CardHead, PctChip } from "../components/shared";
import { Donut } from "../charts";

export function Investments() {
  const { holdings } = useStore();
  const sorted = [...holdings].sort((a, b) => holdingValue(b) - holdingValue(a));
  const totalValue = sorted.reduce((s, h) => s + holdingValue(h), 0);
  const totalCost = sorted.reduce((s, h) => s + holdingCost(h), 0);
  const totalGain = totalValue - totalCost;
  const totalGainPct = totalCost <= 0 ? 0 : (totalGain / totalCost) * 100;
  const segs = sorted.map((h) => ({ label: h.symbol, value: holdingValue(h), color: h.color }));

  return (
    <div className="page">
      <div className="hero invest-hero">
        <div className="hero-left">
          <span className="hero-label">Portfolio value</span>
          <div className="hero-value">{F.currency(totalValue)}</div>
          <div className="hero-meta">
            <PctChip value={totalGainPct} />
            <span className={totalGain >= 0 ? "hero-meta-text pos" : "hero-meta-text neg"}>{F.currency(totalGain)} all-time</span>
          </div>
        </div>
        <div className="hero-donut">
          <Donut size={150} thickness={20} centerLabel={F.currencyCompact(totalValue)} centerSub="invested" segments={segs} />
        </div>
      </div>
      <div className="grid-2">
        <Card>
          <CardHead title="Holdings" right={<span className="muted-sm">{sorted.length} positions</span>} />
          <div className="holding-list">
            {sorted.map((h) => (
              <div className="holding-row" key={h.symbol}>
                <div className="holding-sym" style={{ background: h.color + "22", color: h.color }}>{h.symbol}</div>
                <div className="holding-main">
                  <span className="tx-merchant">{h.name}</span>
                  <span className="tx-sub">{h.shares} sh · {F.currency(h.price)}</span>
                </div>
                <div className="holding-right">
                  <span className="holding-val">{F.currency0(holdingValue(h))}</span>
                  <PctChip value={holdingGainPct(h)} />
                </div>
              </div>
            ))}
          </div>
        </Card>
        <div className="stack">
          <Card>
            <CardHead title="Allocation" />
            <div className="alloc-list">
              {sorted.map((h) => {
                const pct = totalValue <= 0 ? 0 : (holdingValue(h) / totalValue) * 100;
                return (
                  <div className="alloc-row" key={h.symbol}>
                    <span className="alloc-sym">{h.symbol}</span>
                    <div className="alloc-track"><div className="alloc-fill" style={{ width: `${pct}%`, background: h.color }} /></div>
                    <span className="alloc-pct">{F.percent(pct)}</span>
                  </div>
                );
              })}
            </div>
          </Card>
          <Card>
            <CardHead title="Summary" />
            <div className="kv-list">
              <div className="kv"><span>Cost basis</span><span className="kv-val">{F.currency0(totalCost)}</span></div>
              <div className="kv"><span>Market value</span><span className="kv-val">{F.currency0(totalValue)}</span></div>
              <div className="kv"><span>Total return</span><span className={totalGain >= 0 ? "kv-val pos" : "kv-val neg"}>{F.currency0(totalGain)} ({F.percentSigned(totalGainPct)})</span></div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
