# Autosave — never wonder whether your work made it to GitHub

This repo carries a small, version-controlled bundle that makes **every Claude
Code session commit and push its work automatically**. Once it's in a repo, you
never have to remember to save — when a session ends, the work is already on
GitHub.

## What's in the bundle

| File | Purpose |
|------|---------|
| `.claude/settings.json` | Registers a `Stop` hook that fires after every turn. |
| `.claude/hooks/auto-checkpoint.sh` | Commits all changes and pushes them to a `claude/…` branch. |
| `.claude/CLAUDE.md` | The working agreement: auto-persist, keep `main` clean, name repos sensibly. |
| `.gitignore` | Keeps secrets (`.env`, keys, tokens) out of commits. |
| `install-autosave.sh` | One-command installer to add all of the above to any other repo. |

## How it works

1. Claude Code fires a `Stop` hook at the end of **every** assistant turn.
2. The hook checks for changes. If the tree is clean, it does nothing.
3. Otherwise it commits everything with a `chore(autosave): checkpoint …`
   message and pushes to the current `claude/…` branch (creating one if the
   session is somehow on `main`).
4. It runs **alongside** the platform's built-in git-check gate, which already
   refuses to let a turn end with unpushed work — so this turns that gate from a
   reminder into an already-satisfied check.

### Safety properties

- **Never breaks a session** — the hook always exits 0.
- **Verified commits** — it preserves the `Claude <noreply@anthropic.com>`
  signing identity, so pushed commits stay "Verified" on GitHub.
- **Never touches `main`** — protected branches are left alone; work lands on a
  `claude/…` branch.
- **Secret-aware** — `.gitignore` plus a second-line filter keep `.env`, private
  keys, and credential files from being auto-committed.

## Add it to another repo

From inside any git repo:

```bash
bash install-autosave.sh   # copy this script over, or curl it from here
git commit -m "chore: add Claude Code autosave"
git push
```

That repo now auto-saves in every future session — nothing else to configure.

## Notes & trade-offs

- Checkpoints are frequent (one per turn), so the branch accumulates
  `checkpoint` commits. Squash them when you open a PR if you want tidy history.
- User-level `~/.claude/settings.json` does **not** carry into cloud sessions —
  only the committed, in-repo `.claude/` does. That's why this lives in the repo.
- A brand-new project with no repo yet: create the repo first (with a
  descriptive name), then run the installer.
