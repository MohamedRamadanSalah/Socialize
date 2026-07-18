# Contract: Progress Record File Shape

`progress-record.md` is the single persisted file (FR-017) the coach reads at the start of every session and updates immediately after every Evaluation or recorded skip. Exactly one instance exists per engagement (one trainee, per spec Assumptions). Its literal Markdown shape:

```markdown
# Flutter Coaching Progress Record

**Trainee**: <name or identifier>
**Engagement started**: <date>
**Last updated**: <date, updated on every write>
**Current unlocked slice**: <slice name>

## Slice Status

| Slice | Sequence | Status | Last Evaluated | Notes |
|---|---|---|---|---|
| Authentication & Sessions | 1 | not_started \| in_revision \| passed | <date or —> | |
| Profiles & Social Graph | 2 | ... | | |
| Posts & Feed | 3 | ... | | |
| Engagement (Likes/Comments) | 4 | ... | | |
| Notifications & Real-time | 5 | ... | | |
| Search | 6 | ... | | |

## Skip Deviations

<"None recorded." if empty, otherwise one bullet per skip:>
- Skipped from `<slice>` to `<slice>` on <date>, without a passing evaluation on `<slice>`.

## Recurring Violation Patterns

<"None flagged yet." if empty, otherwise one bullet per distinct pattern:>
- **<short pattern name>** — first flagged in `<slice>`; recurred <N> time(s), most recently in `<slice>`.
```

**Read/write rules for the coach**:
- At the start of a session, read this file before doing anything else that depends on gating state (deciding which brief to hand over, whether to reference an earlier violation).
- After every Evaluation: update the evaluated slice's `Status`/`Last Evaluated`, and if `overall_verdict = pass`, advance `Current unlocked slice` to the next slice in sequence (unless already advanced by a skip).
- After every recorded skip (FR-014): append to `Skip Deviations` and update `Current unlocked slice` to the skipped-to slice, leaving the skipped-over slice's status unchanged (still `not_started` or `in_revision`).
- Whenever a cited violation (from `contracts/evaluation-rubric.md`) matches an existing `Recurring Violation Patterns` entry by pattern name, increment its recurrence count and update "most recently in" rather than adding a duplicate bullet (FR-016).
- If the file does not yet exist (first session), create it with all 6 slices `not_started`, `Current unlocked slice` = Authentication & Sessions, both list sections empty.
