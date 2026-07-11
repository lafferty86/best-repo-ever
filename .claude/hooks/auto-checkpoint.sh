#!/usr/bin/env bash
# .claude/hooks/auto-checkpoint.sh
#
# Auto-save: commit & push every change after each Claude Code turn.
# Registered as a `Stop` hook. It runs ALONGSIDE the platform's own
# git-check gate (it does not replace it), turning that gate from a
# "please commit" nag into an already-satisfied check.
#
# Design rules:
#   - Never break a session: this script always exits 0.
#   - Preserve the signing identity (Claude <noreply@anthropic.com>) so
#     pushed commits stay "Verified" on GitHub.
#   - Never auto-commit onto a protected branch, and never auto-commit
#     files that look like secrets.
set +e

# Honor the harness recursion guard if it is passed on stdin.
input="$(cat 2>/dev/null)"
if [ -n "$input" ] && command -v jq >/dev/null 2>&1; then
  if [ "$(printf '%s' "$input" | jq -r '.stop_hook_active // false' 2>/dev/null)" = "true" ]; then
    exit 0
  fi
fi

# Must be inside a git repo, with a remote to push to.
git rev-parse --is-inside-work-tree >/dev/null 2>&1 || exit 0
cd "$(git rev-parse --show-toplevel 2>/dev/null)" 2>/dev/null || exit 0
[ -n "$(git remote 2>/dev/null)" ] || exit 0

# Nothing changed → nothing to do.
if git diff --quiet && git diff --cached --quiet \
   && [ -z "$(git ls-files --others --exclude-standard)" ]; then
  exit 0
fi

# Keep the signing identity; only fall back if it is somehow unset.
git config user.email >/dev/null 2>&1 || git config user.email "noreply@anthropic.com"
git config user.name  >/dev/null 2>&1 || git config user.name  "Claude"

# Never auto-commit straight onto a protected branch — carve a session branch.
branch="$(git rev-parse --abbrev-ref HEAD 2>/dev/null)"
case "$branch" in
  main|master|develop|trunk|"")
    branch="claude/autosave-$(date +%Y%m%d-%H%M%S)"
    git checkout -b "$branch" >/dev/null 2>&1 || exit 0
    ;;
esac

git add -A

# Belt-and-suspenders: unstage anything that looks like a secret.
# (.gitignore is the primary defense; this is a second line.)
for f in $(git diff --cached --name-only 2>/dev/null); do
  case "$(basename "$f")" in
    .env.example|.env.sample|.env.template) continue ;;
  esac
  case "$f" in
    *.pem|*.key|*.p12|*.pfx|*.keystore|*.jks|\
    id_rsa|id_dsa|id_ecdsa|id_ed25519|\
    .env|.env.*|credentials.json|*.credentials.json|secrets.json|*.secrets.json)
      git reset -q -- "$f" >/dev/null 2>&1 ;;
  esac
done

# Nothing left staged after filtering → done.
git diff --cached --quiet && exit 0

git commit -q -m "chore(autosave): checkpoint $(date -u +%Y-%m-%dT%H:%M:%SZ)" >/dev/null 2>&1 || exit 0

# Push with exponential backoff on transient network failures.
n=0
until [ "$n" -ge 4 ]; do
  git push -u origin "$branch" >/dev/null 2>&1 && exit 0
  n=$((n + 1))
  sleep "$((2 ** n))"
done
exit 0
