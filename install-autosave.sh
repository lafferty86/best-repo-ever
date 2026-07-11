#!/usr/bin/env bash
# install-autosave.sh
# Drop the Claude Code autosave bundle into the current git repository.
# Usage:  bash install-autosave.sh      (run from anywhere inside the repo)
#
# It writes .claude/settings.json, .claude/hooks/auto-checkpoint.sh, and a
# .gitignore secrets block, then stages them. Safe to re-run (idempotent).
set -euo pipefail

root="$(git rev-parse --show-toplevel 2>/dev/null)" || {
  echo "Not inside a git repository." >&2; exit 1;
}
mkdir -p "$root/.claude/hooks"

cat > "$root/.claude/hooks/auto-checkpoint.sh" <<'HOOK'
#!/usr/bin/env bash
# Auto-save: commit & push every change after each Claude Code turn.
set +e
input="$(cat 2>/dev/null)"
if [ -n "$input" ] && command -v jq >/dev/null 2>&1; then
  if [ "$(printf '%s' "$input" | jq -r '.stop_hook_active // false' 2>/dev/null)" = "true" ]; then
    exit 0
  fi
fi
git rev-parse --is-inside-work-tree >/dev/null 2>&1 || exit 0
cd "$(git rev-parse --show-toplevel 2>/dev/null)" 2>/dev/null || exit 0
[ -n "$(git remote 2>/dev/null)" ] || exit 0
if git diff --quiet && git diff --cached --quiet \
   && [ -z "$(git ls-files --others --exclude-standard)" ]; then
  exit 0
fi
git config user.email >/dev/null 2>&1 || git config user.email "noreply@anthropic.com"
git config user.name  >/dev/null 2>&1 || git config user.name  "Claude"
branch="$(git rev-parse --abbrev-ref HEAD 2>/dev/null)"
case "$branch" in
  main|master|develop|trunk|"")
    branch="claude/autosave-$(date +%Y%m%d-%H%M%S)"
    git checkout -b "$branch" >/dev/null 2>&1 || exit 0 ;;
esac
git add -A
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
git diff --cached --quiet && exit 0
git commit -q -m "chore(autosave): checkpoint $(date -u +%Y-%m-%dT%H:%M:%SZ)" >/dev/null 2>&1 || exit 0
n=0
until [ "$n" -ge 4 ]; do
  git push -u origin "$branch" >/dev/null 2>&1 && exit 0
  n=$((n + 1)); sleep "$((2 ** n))"
done
exit 0
HOOK
chmod +x "$root/.claude/hooks/auto-checkpoint.sh"

cat > "$root/.claude/settings.json" <<'JSON'
{
  "$schema": "https://json.schemastore.org/claude-code-settings.json",
  "hooks": {
    "Stop": [
      {
        "matcher": "",
        "hooks": [
          {
            "type": "command",
            "command": "bash \"$CLAUDE_PROJECT_DIR/.claude/hooks/auto-checkpoint.sh\""
          }
        ]
      }
    ]
  }
}
JSON

# Append the secrets block to .gitignore if it isn't already there.
if ! grep -q "Secrets & credentials (never commit these)" "$root/.gitignore" 2>/dev/null; then
  cat >> "$root/.gitignore" <<'IGN'

# --- Secrets & credentials (never commit these) ---
.env
.env.*
!.env.example
!.env.sample
!.env.template
*.pem
*.key
*.p12
*.pfx
*.keystore
*.jks
id_rsa
id_dsa
id_ecdsa
id_ed25519
credentials.json
*.credentials.json
secrets.json
*.secrets.json
IGN
fi

git -C "$root" add .claude .gitignore >/dev/null 2>&1 || true
echo "Autosave installed in $root/.claude — commit it, and every future session auto-saves."
