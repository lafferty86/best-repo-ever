# Contributing — GitHub Flow

## The workflow

1. **Branch** — create a branch from `main` with a short descriptive name.
   ```
   git checkout -b my-feature
   ```

2. **Commit** — make small, focused commits as you work.
   ```
   git add <files>
   git commit -m "describe why, not what"
   ```

3. **Push** — push your branch to the remote early and often.
   ```
   git push -u origin my-feature
   ```

4. **Pull Request** — open a PR against `main` when ready for review (or earlier, as a draft, to get early feedback).

5. **Review** — address feedback with new commits on the same branch.

6. **Merge** — once approved, merge into `main`. Delete the branch after merging.

7. **Deploy** — `main` should always be in a deployable state.

## Rules

- `main` is always stable and deployable — never commit broken code directly to it.
- Branch names should be short and meaningful: `fix-login-bug`, `add-search`, `update-deps`.
- Keep PRs small and focused. One concern per PR is easier to review and safer to merge.
- Write commit messages that explain *why* the change was made, not just what changed.
- Delete merged branches to keep the branch list clean.
