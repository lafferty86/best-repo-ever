// Formatting helpers shared across every Money Pilot view.

const group = (n: number): string =>
  Math.round(n).toLocaleString("en-US");

/** "$1,234.56" */
export function currency(value: number): string {
  const totalCents = Math.round(Math.abs(value) * 100);
  const sign = value < 0 && totalCents > 0 ? "-" : "";
  const whole = Math.floor(totalCents / 100);
  const cents = totalCents % 100;
  return `${sign}$${group(whole)}.${cents.toString().padStart(2, "0")}`;
}

/** "$1,235" — no decimals, rounded. */
export function currency0(value: number): string {
  const rounded = Math.round(Math.abs(value));
  const sign = value < 0 && rounded > 0 ? "-" : "";
  return `${sign}$${group(rounded)}`;
}

/** Compact currency for large numbers: "$1.2M", "$12.4k". */
export function currencyCompact(value: number): string {
  const sign = value < 0 ? "-" : "";
  const v = Math.abs(value);
  if (v >= 1_000_000) return `${sign}$${(v / 1_000_000).toFixed(1)}M`;
  if (v >= 1_000) return `${sign}$${(v / 1_000).toFixed(1)}k`;
  return `${sign}$${v.toFixed(0)}`;
}

/** "+3.4%" / "-1.2%" */
export function percentSigned(value: number): string {
  return `${value >= 0 ? "+" : ""}${value.toFixed(1)}%`;
}

export const percent = (value: number): string => `${Math.round(value)}%`;

const MONTHS = [
  "Jan", "Feb", "Mar", "Apr", "May", "Jun",
  "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
];

/** "Jul 12" from an ISO yyyy-MM-dd string (purely lexical, no timezone maths). */
export function shortDate(iso: string): string {
  const parts = iso.split("-");
  if (parts.length !== 3) return iso;
  const mi = parseInt(parts[1], 10) - 1;
  return mi >= 0 && mi < 12 ? `${MONTHS[mi]} ${parseInt(parts[2], 10)}` : iso;
}

/** "Dec 2026" from a yyyy-MM string. */
export function monthYear(ym: string): string {
  const parts = ym.split("-");
  if (parts.length < 2) return ym;
  const mi = parseInt(parts[1], 10) - 1;
  return mi >= 0 && mi < 12 ? `${MONTHS[mi]} ${parts[0]}` : ym;
}

/** Two-letter monogram for an account or merchant. */
export function initials(name: string): string {
  const parts = name.split(/[\s\-_]+/).filter(Boolean);
  if (parts.length === 0) return "?";
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return (parts[0][0] + parts[1][0]).toUpperCase();
}
