# Project working agreement

## Auto-persistence (never lose work)
A `Stop` hook — `.claude/hooks/auto-checkpoint.sh` — commits and pushes every
change after each turn, on a `claude/…` branch. It runs alongside the
platform's own git-check gate; do not disable either. The result: when a
session ends, its work is already on GitHub — nothing to wonder about.

## Repositories & naming
- All work belongs in a GitHub repo. If work outgrows this repo or doesn't fit
  it, create a **new repo early** with a clear, descriptive, kebab-case name
  that says what it is (e.g. `invoice-parser`, `budget-dashboard`,
  `money-pilot`) — never `test`, `tmp`, `best-repo-ever`, or other throwaway
  names.
- Keep `main` clean and deployable. Do real work on `claude/…` branches and
  open a PR when a change is ready for review.

## Hygiene
- Never commit secrets (`.env`, private keys, tokens, credentials) — see
  `.gitignore`.
- Write commit messages that explain *why*, not just *what*.
