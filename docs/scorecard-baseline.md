# OpenSSF Scorecard baseline

The repository is scored weekly, on every push to `main` and on branch-protection changes by
`.github/workflows/scorecard.yaml`. Findings go to the Security tab (Code Scanning), and the score is meant to be
published to [scorecard.dev](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/In-memory-Logger)
and shown by the badge in the README.

## State on 2026-10-22 (`main` at `998877c`)

**No score is published yet.** `https://api.scorecard.dev/projects/github.com/Chris-Wolfgang/In-memory-Logger`
returns 404, so the README badge reads "invalid repo path". scorecard.dev rejected every publish because the
analysis job contained a `run:` step (#299); the fix moves the SARIF filter to its own job. The first push to
`main` after that fix publishes the first score.

What the Code Scanning results show in the meantime (they are the same checks, after the suppressions listed below):

| Check | Score | Why, and what is done about it |
|---|---:|---|
| Branch-Protection | 4 | "Not maximal" on the default branch (no required approvers, no code-owner review). Improving it is a repository settings (ruleset) change, not a code change. Tracked in #211 (`blocked`). |
| Fuzzing | 0 | "No fuzzer integrations found": Scorecard has no detector for .NET property-based testing, so the FsCheck fuzz properties and weekly fuzz workflow added for #105 do not count either. Tracked in #209. |
| Security-Policy | pass | Alert closed 2026-09-23 once `SECURITY.md` met the check. |

Suppressed before the SARIF reaches Code Scanning, with the reason in `scorecard.yaml` (the published score is
unaffected): Code-Review (single-maintainer repository; a required second approver would force an administrator
bypass on every merge), CII-Best-Practices (an external bestpractices.dev registration CI cannot earn), and
Pinned-Dependencies findings for NuGet `restore` commands only (exact versions are pinned in the project files and
nuget.org versions are immutable).

## Floor

Once the first score is published, record it in a table here, one row per check, and treat it as the floor: a
change that would lower the published score below it says why in its pull request. There is no automated
enforcement; the workflow publishes the score but does not compare it with a threshold, and the alerts it raises
are tracked as issues by the security-alerts workflow. Re-measure after each release, since the score depends on
the day's repository state (for example the share of recent changesets that were reviewed).
