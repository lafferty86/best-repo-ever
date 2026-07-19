# Flight Deal Watch

Price log for Google Flights trackers, fed by the alert emails Google sends to
Lafferty's Gmail. A scheduled Claude session checks for new alert emails every
few hours, appends new price points to `prices.csv`, and sends a push
notification when a price qualifies as a dip.

## Watched routes

| Route | Trip dates | Status |
|---|---|---|
| MSP → PDX (Minneapolis → Portland) | Aug 1 – Aug 6, 2026, round trip, 1 adult | Active (already booked — see rebook note) |
| MSP → DC area | Multiple exact-date trackers, dates flexible | Active — grouped, see below |
| SEA → MSP | Feb 6 – Feb 8, 2026 | Expired (trip passed, not logged) |

### Route groups (flexible dates)

Google Flights trackers require exact dates, so a flexible trip is represented
by several trackers with different date pairs. The watcher treats all trackers
to the same destination area as ONE trip group:

- **DC trip** (husband + son): every tracker to DCA/IAD/BWI belongs to this
  group, whatever the dates. Log each variant as its own route
  (e.g. `MSP-DCA 2026-09-12/2026-09-16`), but evaluate dips at the group
  level — what matters is the cheapest variant right now, not each date pair
  in isolation.

To watch a new route: search it on Google Flights while signed in, toggle
**Track prices**, and the watcher will pick up the alert emails automatically.

## Dip rule (when a push notification fires)

A new price triggers a push notification when any of these hold:

1. It is **≥15% below the trailing 14-day median** for the route (or its
   route group's cheapest-variant series).
2. It is an **all-time low** for the route (or group) in this log.
3. It is below a per-route **target price** listed here (none set yet).

Group rules for flexible trips (e.g. the DC trip):

- Evaluate against the **cheapest variant in the group**. A dip in one date
  pair only matters if it beats or approaches the group's best price.
- Notifications should name the winning dates and compare the alternatives,
  e.g. "DC: Sep 12–16 now $312 — cheapest option (others $389 / $404)".
- A route's **first email just seeds its baseline** — don't alert on it unless
  it's a target-price hit or immediately becomes the group's cheapest by ≥15%.

MSP→PDX rebook note: this trip is already booked. Alerts for it are only
useful below the purchase price (rebook for airline credit). Purchase price
not yet recorded — until it is, keep the standard dip rule.

Otherwise the watcher logs the price silently.

## Log format

`prices.csv` columns: `date_utc, route, trip_dates, price_usd, prev_price_usd, email_id`
— one row per Google alert email, `email_id` is the Gmail message id (dedupe key).
