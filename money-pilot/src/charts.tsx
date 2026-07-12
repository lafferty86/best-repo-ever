// Lightweight, dependency-free SVG charts.

import { currency0 } from "./format";

export interface Segment { label: string; value: number; color: string; }

/** Donut / ring chart via the stroke-dasharray technique — crisp at any size. */
export function Donut({
  size, thickness, centerLabel, centerSub, segments,
}: {
  size: number; thickness: number; centerLabel: string; centerSub: string; segments: Segment[];
}) {
  const r = size / 2 - thickness / 2;
  const c = size / 2;
  const circ = 2 * Math.PI * r;
  const total = segments.reduce((s, seg) => s + seg.value, 0);
  let acc = 0;
  const arcs = segments
    .filter((s) => s.value > 0)
    .map((s, i) => {
      const frac = total <= 0 ? 0 : s.value / total;
      const len = frac * circ;
      const offset = -acc;
      acc += len;
      return (
        <circle
          key={i} cx={c} cy={c} r={r} fill="none" stroke={s.color}
          strokeWidth={thickness} strokeLinecap="round"
          strokeDasharray={`${len} ${circ - len}`} strokeDashoffset={offset}
          transform={`rotate(-90 ${c} ${c})`}
          style={{ transition: "stroke-dasharray .6s ease, stroke-dashoffset .6s ease" }}
        />
      );
    });
  return (
    <svg viewBox={`0 0 ${size} ${size}`} width={size} height={size}>
      <circle cx={c} cy={c} r={r} fill="none" stroke="var(--track)" strokeWidth={thickness} />
      {arcs}
      <text x={c} y={c - 4} textAnchor="middle" dominantBaseline="middle"
        style={{ fontWeight: 800, fontSize: 18, fill: "var(--text)" }}>{centerLabel}</text>
      <text x={c} y={c + 16} textAnchor="middle" dominantBaseline="middle"
        style={{ fontWeight: 600, fontSize: 10, fill: "var(--muted)", letterSpacing: ".06em" }}>
        {centerSub.toUpperCase()}
      </text>
    </svg>
  );
}

/** Smooth area + line chart. Values map to evenly-spaced x positions. */
export function AreaLine({
  w, h, color, gradId, values,
}: { w: number; h: number; color: string; gradId: string; values: number[]; }) {
  if (values.length === 0) return null;
  const pad = 8;
  const min = Math.min(...values);
  const max = Math.max(...values);
  const range = max - min === 0 ? 1 : max - min;
  const n = values.length;
  const stepX = n <= 1 ? 0 : (w - pad * 2) / (n - 1);
  const pts = values.map((v, i) => [
    pad + stepX * i,
    h - pad - ((v - min) / range) * (h - pad * 2),
  ] as const);
  const lineD = pts.map(([x, y], i) => `${i === 0 ? "M" : "L"}${x.toFixed(2)} ${y.toFixed(2)}`).join(" ");
  const areaD = `${lineD} L${pts[pts.length - 1][0].toFixed(2)} ${h - pad} L${pts[0][0].toFixed(2)} ${h - pad} Z`;
  return (
    <svg viewBox={`0 0 ${w} ${h}`} width="100%" height={h} preserveAspectRatio="none">
      <defs>
        <linearGradient id={gradId} x1="0" y1="0" x2="0" y2="1">
          <stop offset="0" stopColor={color} stopOpacity="0.35" />
          <stop offset="1" stopColor={color} stopOpacity="0" />
        </linearGradient>
      </defs>
      <path d={areaD} fill={`url(#${gradId})`} />
      <path d={lineD} fill="none" stroke={color} strokeWidth={2.5} strokeLinecap="round" strokeLinejoin="round" />
      {pts.map(([x, y], i) => <circle key={i} cx={x} cy={y} r={2.6} fill={color} />)}
    </svg>
  );
}

/** Grouped vertical bars — income vs. expense per month. */
export function GroupedBars({ h, rows }: { h: number; rows: [string, number, number][]; }) {
  const all = rows.flatMap(([, a, b]) => [a, b]);
  const maxV = all.length ? Math.max(...all) : 1;
  const barH = (v: number) => (maxV <= 0 ? 0 : (v / maxV) * h);
  return (
    <div className="gbars">
      {rows.map(([label, income, expense]) => (
        <div className="gbar-col" key={label}>
          <div className="gbar-pair">
            <div className="gbar income" style={{ height: barH(income) }} title={currency0(income)} />
            <div className="gbar expense" style={{ height: barH(expense) }} title={currency0(expense)} />
          </div>
          <span className="gbar-label">{label}</span>
        </div>
      ))}
    </div>
  );
}

export interface RankedRow { icon: string; label: string; value: number; color: string; }

/** Horizontal ranked bars — e.g. top spending categories. */
export function RankedBars({ rows, onClick }: { rows: RankedRow[]; onClick?: (label: string) => void; }) {
  const maxV = rows.length ? Math.max(...rows.map((r) => r.value)) : 1;
  return (
    <div className="rbars">
      {rows.map((r) => (
        <div
          className="rbar-row" key={r.label}
          onClick={onClick ? () => onClick(r.label) : undefined}
          style={onClick ? { cursor: "pointer" } : undefined}
        >
          <span className="rbar-icon">{r.icon}</span>
          <div className="rbar-body">
            <div className="rbar-head">
              <span className="rbar-name">{r.label}</span>
              <span className="rbar-val">{currency0(r.value)}</span>
            </div>
            <div className="rbar-track">
              <div className="rbar-fill" style={{ width: `${maxV <= 0 ? 0 : (r.value / maxV) * 100}%`, background: r.color }} />
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
