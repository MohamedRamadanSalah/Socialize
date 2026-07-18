# Contract: Evaluation Rubric

Applied by the coach to every explicit Submission (FR-009). All four dimensions are evaluated on every submission; overall pass requires all four to pass (Story 5, Scenario 3) plus a satisfactory design explanation (FR-012).

## Dimension 1 — Functional Correctness

Checked against the fixed backend contract in `001-social-media-backend` (`backend/API.md` / `contracts/`), never against the coach's own guess of what the API should do.

- Does the slice call the correct endpoint(s), with correct methods/paths/payloads?
- Are the documented success and error responses handled (not just the happy path)?
- Do the acceptance criteria in this slice's task brief actually pass when exercised?

**Fails when**: a documented endpoint is missed, misused, or its documented error shape is ignored; an acceptance criterion doesn't hold.

## Dimension 2 — Clean Architecture Compliance

- Domain layer has zero imports of Flutter, Riverpod, or HTTP/networking packages.
- Data layer implements domain-defined repository interfaces; the domain never imports the data layer.
- Presentation layer talks to the domain via providers, never directly to a data source or HTTP client.
- Dependency direction is inward only: presentation → domain ← data.
- The slice is self-contained (feature-first): it does not require modifying a "shared everything" layer that couples it to other slices.

**Fails when**: any of the above is violated, *regardless of whether the app compiles or its tests pass* (FR-011) — automated-check success is never sufficient on its own.

## Dimension 3 — Riverpod Usage Correctness

- Provider types match their purpose (e.g. async server data via `FutureProvider`/`AsyncNotifierProvider`, not manual `setState`-style flags).
- No business logic lives inside a widget's `build` method or event handlers — it lives behind a provider.
- Provider scope matches lifetime needs (e.g. auth session state isn't accidentally re-created per screen).
- Async/error/loading states are modeled explicitly (e.g. via `AsyncValue`), not swallowed or ignored.

**Fails when**: widgets contain business logic, provider types don't match their data's shape, or error/loading states are unhandled — again, independent of whether the code happens to run.

## Dimension 4 — Code Quality

- Naming and structure are consistent with the rest of the trainee's own project (not perfection — consistency).
- No obviously dead code, commented-out blocks, or copy-pasted boilerplate left behind.
- Error handling doesn't silently swallow failures the user should see.

## Overall Verdict Rule

```text
overall_verdict = pass
  IFF all four dimension_verdicts = pass
  AND trainee_explanation is present and demonstrates real understanding (FR-012)
ELSE
  overall_verdict = needs_revision
```

## Output Shape (every evaluation, regardless of verdict)

1. Per-dimension verdict (pass / needs_revision).
2. For every `needs_revision` dimension: specific cited violation(s) — what, and where in the trainee's own description of their code (never a rewritten version of their code, per FR-006/FR-010).
3. Guiding next steps as questions or concepts to revisit — never a fix, snippet, or diff.
4. If a cited violation matches an entry already in `progress-record.md`'s `violation_history`, the feedback explicitly references the earlier occurrence (FR-016).
