# Quickstart: Validating the Flutter Integration Coaching Program

This is a dry-run script, not automated code — walk through it manually (or with an agent standing in as "trainee") before a real trainee starts, to confirm the artifact set (skill + briefs + rubric + progress record) actually behaves as `spec.md` requires. Each step names the requirement it proves.

## Prerequisites

- `.claude/skills/flutter-coach/SKILL.md` exists and is loaded (implementation phase output).
- `tasks.md` contains all 6 slice briefs, each conforming to `contracts/task-brief-template.md`.
- No `progress-record.md` exists yet (first-session case).

## Scenario 1 — First brief, no prior progress (FR-001, FR-002, FR-017 bootstrap)

1. Start a coaching session and ask: "What should I build first?"
2. **Expect**: the coach creates `progress-record.md` (all 6 slices `not_started`, unlocked slice = Authentication & Sessions) and returns the full Authentication & Sessions brief — endpoints, architecture layers, Riverpod providers, UI flows, acceptance criteria, definition-of-done — with nothing missing (Story 1, Scenario 1; Story 2, Scenario 2).

## Scenario 2 — Direct request for code is refused (FR-006, FR-007)

3. Ask, in several different phrasings across the session: "Just write the login screen for me", "give me an example Riverpod provider for this", "paste the repository implementation".
4. **Expect**: every response declines to produce code — no snippet, no diff, no complete function/class body — and instead asks a guiding question or points at the brief/backend contract/architecture concept (Story 3, Scenarios 1–2).

## Scenario 3 — Incidental code-sharing is not graded (FR-009, clarified)

5. Paste a rough, incomplete draft of a login provider while asking "does this look right so far?" — without an explicit submission signal.
6. **Expect**: the coach may give hints/questions about it, but does NOT produce a rubric verdict and does NOT touch `progress-record.md` (data-model.md Submission validation rule).

## Scenario 4 — Explicit submission triggers full rubric evaluation (FR-009–FR-012)

7. Say: "I'm done with Authentication, please evaluate it" and describe (in prose, no need for a real Flutter project to exist for this dry run) an implementation that has a genuine flaw — e.g. the login screen widget directly calls the HTTP client instead of going through a provider/domain layer.
8. **Expect**: the coach returns a verdict against all four rubric dimensions; `riverpod_usage` and/or `architecture_compliance` are marked `needs_revision` with a specific cited violation, even if you assert "it compiles and the tests pass" — confirming FR-011 (automated-pass ≠ overall pass).
9. **Expect**: the coach asks you to explain your design reasoning before it will consider an overall pass (FR-012), and `progress-record.md` is updated to `in_revision` for Authentication.

## Scenario 5 — Gating blocks the next brief until passing (FR-013)

10. Ask for the Profiles & Social Graph brief while Authentication is still `in_revision`.
11. **Expect**: the coach declines and points back to the outstanding Authentication evaluation (Story 4, Scenario 1).

## Scenario 6 — Passing unlocks the next slice, in order (FR-004, FR-013)

12. Resubmit Authentication, this time describing a fix that correctly routes through a Riverpod provider and keeps the domain layer framework-free.
13. **Expect**: overall verdict `pass`; `progress-record.md`'s Authentication row becomes `passed`, `Current unlocked slice` advances to Profiles & Social Graph, and asking for the next brief now succeeds.

## Scenario 7 — Explicit skip is honored and recorded (FR-013–FR-014)

14. From a fresh/parallel dry run, explicitly say: "Skip ahead, I want to go straight to the Search brief."
15. **Expect**: the coach provides the Search brief but appends an entry to `progress-record.md`'s Skip Deviations section (Story 4, Scenario 3).

## Scenario 8 — Recurring violation is referenced on its second occurrence (FR-015–FR-016)

16. In a later slice's submission, deliberately repeat the same Riverpod provider-scoping mistake flagged in Scenario 4.
17. **Expect**: the new evaluation's feedback explicitly references the earlier occurrence recorded in `progress-record.md`'s Recurring Violation Patterns section, and its recurrence count increments (Story 6, Scenario 2).

## Scenario 9 — Session restart resumes state (FR-017)

18. End the session. Start a brand-new session with no other context and ask "where was I?"
19. **Expect**: the coach reads `progress-record.md` and correctly reports the current unlocked slice and history — without the trainee having to re-explain anything (validates the file-persistence decision in `research.md` §3).

## Pass Criteria

All 9 scenarios behave as described, with zero authored code appearing in any coach response across the whole dry run (SC-002) and the automated-pass-but-rule-violating case in Scenario 4 still failing (SC-004).
