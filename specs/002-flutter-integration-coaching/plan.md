# Implementation Plan: Flutter Integration Coaching Program

**Branch**: `002-flutter-integration-coaching` | **Date**: 2026-07-18 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-flutter-integration-coaching/spec.md`

## Summary

Deliver a coaching program — not a piece of software the trainee runs, but a reusable Claude Code artifact plus a fixed set of process documents — that turns "build a Flutter client against the Socialize backend" into a gated, per-feature, no-code-from-the-coach training path. Six vertical-slice task briefs (Auth, Profiles/Social Graph, Posts/Feed, Engagement, Notifications, Search), one per backend feature area, are authored as the detailed content of `tasks.md` in Phase 2. A fixed evaluation rubric and task-brief template (this plan's contracts) keep every brief and every verdict structurally consistent. The behavioral contract itself — never author code, always evaluate via rubric, gate progression, persist progress — is encoded as a new project skill (`.claude/skills/flutter-coach/SKILL.md`) so any future session loads and enforces it automatically, and a persisted `progress-record.md` file carries gating state and recurring-violation history across sessions.

Technical approach: no runtime service is built. The "system" is a Markdown-based artifact set (skill instructions + task briefs + rubric + progress file) read by Claude at the start of each coaching session, plus a Dart/Flutter target-side convention (Riverpod + Clean Architecture, feature-first folders) that the rubric checks the trainee's actual Flutter project against.

## Technical Context

**Language/Version**: The coaching artifacts themselves are Markdown (Claude Code skill format) — no compiled language. The trainee's target app is Dart (current stable Flutter SDK) — a specific pinned version is the trainee's own environment choice, not fixed by this program (env setup is out of scope per spec Assumptions).

**Primary Dependencies**: Claude Code's project-skill mechanism (`.claude/skills/`) for the coaching contract; no other runtime dependency for the coaching program itself. On the trainee's Flutter side, the rubric mandates `flutter_riverpod`/`riverpod` (with `riverpod_generator` code-gen style, current convention) for state management and a repository-pattern HTTP client (e.g. `dio`) — trainees choose the exact packages, the rubric only checks the pattern.

**Storage**: A single persisted `progress-record.md` file per engagement (`specs/002-flutter-integration-coaching/progress-record.md`), read at the start of a session and updated after every evaluation (FR-017). No database — the file itself is the store, consistent with this being a process artifact rather than a service.

**Testing**: There is no automated test suite for the coaching program itself (it is evaluated by review, not executed). Validation is a scripted dry-run coaching conversation in `quickstart.md` that exercises: brief delivery → refusal-of-code-request → explicit-submission evaluation → gating → progress persistence. Rubric correctness (FR-011: automated-pass-but-architecture-fail still fails) is validated the same way, against a deliberately-flawed sample submission.

**Target Platform**: Claude Code CLI sessions (any developer machine) run the coaching program. The trainee's Flutter app itself targets whatever platforms Flutter supports (Android/iOS/Web); the specific target platform is the trainee's choice and out of scope here.

**Project Type**: Process/coaching artifact — a Claude Code project skill plus companion documents, not a software service or library.

**Performance Goals**: Not applicable in the throughput/latency sense. The relevant goals are the spec's process-compliance metrics: 100% of coach responses contain no authored code (SC-002), ≥95% of sessions respect gating (SC-003), 100% of automated-pass-but-rule-violating submissions are still marked failing (SC-004).

**Constraints**: The coach MUST NOT author code under any framing (FR-006). Evaluation only runs on an explicit trainee submission signal, never on incidental code-sharing (FR-009). Functional correctness in the rubric is judged against the fixed backend contract in `001-social-media-backend` (spec/API.md) — that contract is not renegotiable by this feature. Progress state MUST survive a session ending (FR-017).

**Scale/Scope**: One trainee per engagement (no multi-trainee cohort dashboards). Six vertical slices, one evaluation rubric, one task-brief template, one progress-record file, one coaching skill artifact.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The project constitution (`.specify/memory/constitution.md`) is an unratified template with placeholder principles, so there are **no binding constitutional gates** (same situation noted in `001-social-media-backend/plan.md`). In their absence, this plan treats the spec's own functional requirements as the review criteria:

| Guardrail | Status | Notes |
|-----------|--------|-------|
| Coach never authors code, in any phrasing (FR-006) | PASS | Enforced as a hard rule in the `flutter-coach` skill instructions; exercised in `quickstart.md` |
| Evaluation requires explicit submission signal, never auto-graded (FR-009) | PASS | Skill instructions define the signal convention (see `research.md`) |
| Rubric covers functional + Clean Architecture + Riverpod + quality, cites violations (FR-009–FR-012) | PASS | Fixed structure in `contracts/evaluation-rubric.md` |
| Automated-pass ≠ overall pass when architecture/Riverpod rules are violated (FR-011) | PASS | Explicit rubric dimension, independent of compile/test outcome |
| One self-sufficient brief per feature, in backend priority order (FR-001–FR-005) | PASS | `tasks.md` (Phase 2) authors one detailed section per slice using `contracts/task-brief-template.md` |
| Gated progression + recorded skip deviations (FR-013–FR-014) | PASS | Gating state lives in `progress-record.md`, checked before releasing the next brief |
| Progress + recurring-pattern history persists across sessions (FR-015–FR-017) | PASS | `progress-record.md` read at session start, updated after each evaluation |
| Coaching contract is a reusable, auto-loaded artifact, not per-session re-briefing (FR-018) | PASS | New project skill `.claude/skills/flutter-coach/SKILL.md` |

**Result**: PASS (no violations; Complexity Tracking not required).

## Project Structure

### Documentation (this feature)

```text
specs/002-flutter-integration-coaching/
├── plan.md                          # This file (/speckit-plan output)
├── research.md                      # Phase 0 output
├── data-model.md                    # Phase 1 output — Task Brief, Submission, Evaluation, Progress Record schemas
├── quickstart.md                    # Phase 1 output — scripted dry-run coaching conversation
├── contracts/                       # Phase 1 output — the fixed shapes every brief/verdict/progress file must follow
│   ├── task-brief-template.md       # Structure every vertical-slice brief must follow (FR-002)
│   ├── evaluation-rubric.md         # Fixed rubric dimensions + pass/fail rules (FR-009–FR-012)
│   └── progress-record.schema.md    # Fixed shape of the persisted Progress Record (FR-017)
├── checklists/
│   └── requirements.md              # Spec quality checklist (from /speckit-specify)
└── tasks.md                         # Phase 2 output (/speckit-tasks) — one detailed brief section per slice
```

### Source Code (repository root)

There is no `src/` for this feature — the deliverable is a coaching artifact set, not a compiled program. The one piece that behaves like "source" is the reusable Claude Code skill that encodes the coaching contract (FR-018), plus the persisted progress file (FR-017):

```text
.claude/skills/flutter-coach/
└── SKILL.md                # FR-018 — no-code rule, rubric pointer, gating logic, brief pointer;
                             # auto-loaded by any future session (authored in /speckit-implement,
                             # using contracts/ + tasks.md as its source material)

specs/002-flutter-integration-coaching/
└── progress-record.md      # FR-017 — persisted per-trainee gating state, pass/revision history,
                             # recorded skip deviations, recurring violation patterns
```

The trainee's own Flutter project (built against `backend/API.md`) is a separate, out-of-repo deliverable — this feature does not scaffold it; each vertical-slice brief in `tasks.md` tells the trainee what to create inside their own project (feature-first folders, one `domain/`, `data/`, `presentation/` per slice, Riverpod providers per FR-005), but the coach never creates those files itself (FR-006).

**Structure Decision**: Reuse the existing spec-kit documentation layout (`plan.md`/`research.md`/`data-model.md`/`contracts/`/`quickstart.md`/`tasks.md`) to design the coaching program, exactly as `001-social-media-backend` did to design the backend. The one enforceable runtime artifact this plan adds outside `specs/` is `.claude/skills/flutter-coach/SKILL.md` — consistent with how the repository already ships its other process behavior as project skills (`.claude/skills/speckit-*`). No `frontend/`, `backend/`, or mobile-app scaffold is created by this feature; `backend/` (from `001-social-media-backend`) remains the fixed integration target, and the trainee's Flutter app lives outside this repository's planning scope.

## Implementation Phases (delivery order, verified between each)

`/speckit-tasks` will expand these into concrete tasks; each phase is verifiable by re-reading the produced artifact against its contract before the next begins.

1. **Contracts** — author `contracts/task-brief-template.md`, `contracts/evaluation-rubric.md`, `contracts/progress-record.schema.md` (Phase 1 of this plan).
2. **Task briefs** — author `tasks.md` with one detailed section per vertical slice (Auth → Profiles/Social Graph → Posts/Feed → Engagement → Notifications → Search), each conforming to `task-brief-template.md`.
3. **Coaching skill** — author `.claude/skills/flutter-coach/SKILL.md`, encoding the no-code rule, the submission-signal convention, the rubric (by reference), the gating logic, and the progress-record read/update behavior.
4. **Progress record bootstrap** — create the initial `progress-record.md` (all slices unpassed, Authentication unlocked).
5. **Dry-run validation** — walk `quickstart.md` end to end (brief delivery, code-request refusal, explicit-submission evaluation, gating, progress persistence, recurring-violation callback) to confirm the artifact set behaves as specified before a real trainee starts.

## Complexity Tracking

No constitutional violations to justify. Section intentionally left empty.
