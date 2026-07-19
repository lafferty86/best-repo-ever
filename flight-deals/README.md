# Flight Deal Watch

Price log for Google Flights trackers, fed by the alert emails Google sends to
Lafferty's Gmail. A scheduled Claude session checks for new alert emails every
few hours, appends new price points to `prices.csv`, and sends a push
notification when a price qualifies as a dip.

## Watched routes

| Route | Trip dates | Status |
|---|---|---|
| MSP → PDX (Minneapolis → Portland) | Aug 1 – Aug 6, 2026, round trip, 1 adult | Active |
| SEA → MSP | Feb 6 – Feb 8, 2026 | Expired (trip passed, not logged) |

To watch a new route: search it on Google Flights while signed in, toggle
**Track prices**, and the watcher will pick up the alert emails automatically.

## Dip rule (when a push notification fires)

A new price triggers a push notification when any of these hold:

1. It is **≥15% below the trailing 14-day median** for the route.
2. It is an **all-time low** for the route in this log.
3. It is below a per-route **target price** listed here (none set yet).

Otherwise the watcher logs the price silently.

## Log format

`prices.csv` columns: `date_utc, route, trip_dates, price_usd, prev_price_usd, email_id`
— one row per Google alert email, `email_id` is the Gmail message id (dedupe key).
