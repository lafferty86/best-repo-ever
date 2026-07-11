# Working agreement — GitHub on autopilot, done right

The aim: across every session, GitHub is used in a best-practice way
automatically, with nothing for the human to remember.

## 1. Auto-persistence (never lose work)
A `Stop` hook — `.claude/hooks/auto-checkpoint.sh` — commits and pushes every
change after each turn, on a `claude/…` branch. It runs alongside the
platform's own git-check gate; do not disable either. When a session ends, its
work is already on GitHub — nothing to wonder about.

## 2. Branching (GitHub Flow)
- `main` is always stable and deployable — never commit directly to it.
- Do all real work on a short, descriptive branch: `claude/<what>` or a clear
  name like `fix-login`, `add-search`, `money-pilot`.
- Branch off the latest `main`; push early and often (autosave handles this).

## 3. Commits
- Small, focused commits. Messages explain *why*, not just *what*.
- Prefer conventional prefixes (`feat:`, `fix:`, `chore:`, `docs:`) for clarity.
- Keep the `Claude <noreply@anthropic.com>` identity so commits stay Verified.

## 4. Pull requests & review
- Open a PR when a change is ready for review (or as a draft for early
  feedback). One concern per PR — small PRs are easier to review and safer.
- Address feedback with new commits on the same branch.
- After merge, delete the branch to keep the list clean.
- Do **not** open a PR unless the human asks for one; when you do, follow any
  `PULL_REQUEST_TEMPLATE`.

## 5. Repositories & naming
- All work belongs in a GitHub repo. If work outgrows the current repo or
  doesn't fit it, create a **new repo early** with a clear, descriptive,
  kebab-case name that says what it is (e.g. `invoice-parser`,
  `budget-dashboard`, `money-pilot`) — never `test`, `tmp`, `best-repo-ever`,
  or other throwaway names. Scaffold the autosave bundle into every new repo.

## 6. Hygiene & secrets
- Never commit secrets (`.env`, private keys, tokens, credentials) — see
  `.gitignore`. The autosave hook also refuses to stage obvious secret files.
- Keep history clean; squash noisy `checkpoint` commits when opening a PR.
