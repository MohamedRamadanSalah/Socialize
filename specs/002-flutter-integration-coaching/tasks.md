---
description: "Task list for Flutter Integration Coaching Program"
---

# Tasks: Flutter Integration Coaching Program

**Input**: Design documents from `/specs/002-flutter-integration-coaching/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: This feature produces no compiled software, so there is no automated test suite. "Tests" here means the scripted dry-run scenarios in `quickstart.md`, which each user-story phase runs as its own verification step instead of an automated test task.

**Organization**: Tasks are grouped by user story (from spec.md) to enable independent implementation and verification of each story.

**Note on artifact locations**: `plan.md` described the 6 vertical-slice briefs as living inside `tasks.md`; for checklist-format compliance (every task needs its own file path) they instead live as one file per slice under `specs/002-flutter-integration-coaching/briefs/`, each following `contracts/task-brief-template.md`. This tasks.md remains the checklist of authoring work, not the curriculum content itself.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US6)
- All paths are relative to repo root.

## Path Conventions (from plan.md)

- Coaching skill (the reusable, auto-loaded contract, FR-018): `.claude/skills/flutter-coach/SKILL.md`
- Vertical-slice briefs (FR-001/002, one per feature area): `specs/002-flutter-integration-coaching/briefs/<slice-id>.md`
- Persisted progress state (FR-017): `specs/002-flutter-integration-coaching/progress-record.md`
- Contracts already authored in Phase 1 of `plan.md`: `specs/002-flutter-integration-coaching/contracts/`

---

## Phase 1: Setup

**Purpose**: Create the directories and skeletons the rest of the work fills in.

- [X] T001 Create `specs/002-flutter-integration-coaching/briefs/` directory to hold the 6 vertical-slice brief documents
- [X] T002 [P] Create `.claude/skills/flutter-coach/SKILL.md` with YAML frontmatter only (`name: flutter-coach`, `description`, `argument-hint`, `user-invocable: true`), matching the convention in `.claude/skills/speckit-plan/SKILL.md`
- [X] T003 [P] Confirm all Phase 0/1 design docs are present and internally consistent: `research.md`, `data-model.md`, `contracts/task-brief-template.md`, `contracts/evaluation-rubric.md`, `contracts/progress-record.schema.md`, `quickstart.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The minimum working skill — session bootstrap, brief lookup, progress-file read/write, and the baseline no-code rule — that every user story depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T004 Author the session-bootstrap behavior in `.claude/skills/flutter-coach/SKILL.md`: on invocation, read `specs/002-flutter-integration-coaching/progress-record.md` if present, otherwise create it per `contracts/progress-record.schema.md`; report `current_unlocked_slice` to the trainee
- [X] T005 [P] Author the brief-lookup behavior in `.claude/skills/flutter-coach/SKILL.md`: given `current_unlocked_slice`, load and return the matching file at `specs/002-flutter-integration-coaching/briefs/<slice_id>.md` (depends on T001)
- [X] T006 [P] Create the initial `specs/002-flutter-integration-coaching/progress-record.md` (all 6 slices `not_started`, `current_unlocked_slice` = Authentication & Sessions, empty Skip Deviations and Recurring Violation Patterns sections) per `contracts/progress-record.schema.md`
- [X] T007 Author the baseline no-code refusal rule (FR-006) and Socratic-response instruction (FR-007) in `.claude/skills/flutter-coach/SKILL.md` (depends on T004)
- [X] T008 Wire `.claude/skills/flutter-coach/SKILL.md` to point at `contracts/evaluation-rubric.md` as the single source of truth for grading — no rubric content duplicated inline (depends on T004)

**Checkpoint**: The skill loads, reports current status, hands over whichever brief is unlocked, and refuses direct code requests. No slice content exists yet — user story work can now begin.

---

## Phase 3: User Story 1 - Guided first vertical slice: Authentication (Priority: P1) 🎯 MVP

**Goal**: Prove the full brief → build → submit → evaluate → iterate loop end-to-end using the Authentication & Sessions slice.

**Independent Test**: Give a trainee only the Authentication brief; confirm they can reach a passing evaluation without the coach ever supplying code. Per `quickstart.md` Scenarios 1, 2, 4, 6 (Auth-scoped).

### Implementation for User Story 1

- [X] T009 [US1] Author `specs/002-flutter-integration-coaching/briefs/auth.md` — Authentication & Sessions brief (goal; backend endpoints from `backend/API.md`'s auth section; domain/data/presentation responsibilities; Riverpod providers for session state; UI flows for register/login/logout + silent refresh; acceptance criteria mirroring `001-social-media-backend/spec.md` Story 1; definition-of-done checklist) per `contracts/task-brief-template.md`
- [X] T010 [US1] Author the explicit-submission recognition instruction (FR-009, signal phrases from `research.md` §1) in `.claude/skills/flutter-coach/SKILL.md` (depends on T007)
- [X] T011 [US1] Author the evaluate-and-iterate loop in `.claude/skills/flutter-coach/SKILL.md`: on a recognized submission, apply `contracts/evaluation-rubric.md`, return the structured verdict, and on a failing verdict allow resubmission without ever supplying a fix (depends on T008, T010)
- [X] T012 [US1] Author the pass-advances-progress instruction in `.claude/skills/flutter-coach/SKILL.md`: on a passing evaluation, update the evaluated slice's row in `progress-record.md` to `passed` and advance `current_unlocked_slice` to the next slice per `contracts/progress-record.schema.md` (depends on T006, T011)
- [X] T013 [US1] Run `quickstart.md` Scenarios 1, 2, 4, and 6 (scoped to Authentication) as a dry run; fix any gaps found in `SKILL.md` or `briefs/auth.md`

**Checkpoint**: User Story 1 fully functional and independently testable (MVP).

---

## Phase 4: User Story 2 - A detailed, self-sufficient task brief per backend feature (Priority: P1)

**Goal**: All 6 backend feature areas have a complete, structurally consistent brief that stands alone.

**Independent Test**: Hand any single feature's brief to a trainee with no other context; confirm they can identify endpoints, layers, providers, and how they'll be judged, from the brief alone. Per `quickstart.md`'s brief-completeness expectations (Scenario 1).

### Implementation for User Story 2

- [X] T014 [P] [US2] Author `specs/002-flutter-integration-coaching/briefs/profiles-social-graph.md` per `contracts/task-brief-template.md`, sourced from `001-social-media-backend/spec.md` Story 3 and its profile/follow endpoints
- [X] T015 [P] [US2] Author `specs/002-flutter-integration-coaching/briefs/posts-feed.md` per `contracts/task-brief-template.md`, sourced from `001-social-media-backend/spec.md` Story 4 and its posts/feed endpoints
- [X] T016 [P] [US2] Author `specs/002-flutter-integration-coaching/briefs/engagement.md` per `contracts/task-brief-template.md`, sourced from `001-social-media-backend/spec.md` Story 5 and its likes/comments endpoints
- [X] T017 [P] [US2] Author `specs/002-flutter-integration-coaching/briefs/notifications.md` per `contracts/task-brief-template.md`, sourced from `001-social-media-backend/spec.md` Story 6, its notifications/real-time endpoints, and the real-time client guidance in `research.md` §5
- [X] T018 [P] [US2] Author `specs/002-flutter-integration-coaching/briefs/search.md` per `contracts/task-brief-template.md`, sourced from `001-social-media-backend/spec.md` Story 7 and its search endpoints
- [X] T019 [US2] Cross-check all 6 briefs (`auth.md` plus the 5 above) for structural consistency against `contracts/task-brief-template.md` and correct `sequence_position`/`prerequisite_slice_id` chaining per `data-model.md` (depends on T009, T014, T015, T016, T017, T018)

**Checkpoint**: User Stories 1 and 2 both done — a trainee can read any brief in isolation and know exactly what to build.

---

## Phase 5: User Story 3 - Coach never writes code; only questions, hints, and verdicts (Priority: P1)

**Goal**: Harden and verify the no-code contract against every phrasing, across multiple slices.

**Independent Test**: Ask the coach to "just write the code" in several phrasings across several slices; confirm every response declines and redirects. Per `quickstart.md` Scenarios 2 and 3.

### Implementation for User Story 3

- [X] T020 [US3] Expand the no-code rule in `.claude/skills/flutter-coach/SKILL.md` to explicitly cover indirect phrasings ("give an example implementation", "just this one snippet", "pseudo-code that's basically real code") so FR-006 holds regardless of phrasing (depends on T007)
- [X] T021 [US3] Author the hint-escalation instruction (FR-008) in `.claude/skills/flutter-coach/SKILL.md`: when the trainee is stuck across repeated exchanges on the same point, increase hint specificity (name the exact pattern/provider type/concept) while still never emitting code (depends on T020)
- [X] T022 [US3] Author the incidental-code-vs-submission boundary instruction in `.claude/skills/flutter-coach/SKILL.md`: code shared without an explicit signal gets hints only, never a rubric verdict or a `progress-record.md` update (depends on T010, T020)
- [X] T023 [US3] Run `quickstart.md` Scenarios 2 and 3 as a dry run across at least 3 different slices' briefs and phrasings; fix any leakage found

**Checkpoint**: User Stories 1–3 done — the P1 MVP (workflow, full brief set, hardened no-code contract) is complete.

---

## Phase 6: User Story 4 - Gated progression across vertical slices (Priority: P2)

**Goal**: A trainee cannot reach a later slice without a passing evaluation on the current one, except via an explicit, recorded skip.

**Independent Test**: Attempt to obtain the Posts/Feed brief before Authentication has passed; confirm refusal. Per `quickstart.md` Scenarios 5 and 7.

### Implementation for User Story 4

- [X] T024 [US4] Author the gating-check instruction in `.claude/skills/flutter-coach/SKILL.md`: before handing over any brief other than `current_unlocked_slice`, refuse and point back to the outstanding evaluation (depends on T005)
- [X] T025 [US4] Author the explicit-skip handling instruction in `.claude/skills/flutter-coach/SKILL.md`: recognize an explicit skip request, advance `current_unlocked_slice` to the requested slice, and append a Skip Deviations entry per `contracts/progress-record.schema.md` (depends on T024, T006)
- [X] T026 [US4] Run `quickstart.md` Scenarios 5 and 7 as a dry run; fix any gaps found

**Checkpoint**: Gating and explicit-skip recording both verified.

---

## Phase 7: User Story 5 - Architecture and pattern compliance evaluation (Priority: P2)

**Goal**: Architecture/Riverpod rubric dimensions are enforced independently of automated-check success, with cited violations and a required design explanation.

**Independent Test**: Submit work that runs correctly but violates a named Clean Architecture or Riverpod rule; confirm it is still marked failing with the specific violation cited. Per `quickstart.md` Scenario 4.

### Implementation for User Story 5

- [X] T027 [US5] Author the per-dimension citation instruction in `.claude/skills/flutter-coach/SKILL.md`: every `needs_revision` dimension must include a specific cited violation (what and where), never a rewritten version of the trainee's code (depends on T011)
- [X] T028 [US5] Author the automated-pass-is-not-sufficient rule (FR-011) in `.claude/skills/flutter-coach/SKILL.md`: disregard trainee claims of "it compiles / tests pass" when `architecture_compliance` or `riverpod_usage` are violated (depends on T027)
- [X] T029 [US5] Author the trainee-explanation gate (FR-012) in `.claude/skills/flutter-coach/SKILL.md`: withhold an overall pass until the trainee explains their key design decisions in their own words (depends on T027)
- [X] T030 [US5] Run `quickstart.md` Scenario 4 as a dry run with a deliberately flawed submission; confirm the failing verdict and the explanation gate both hold

**Checkpoint**: Rubric enforcement fully verified end-to-end.

---

## Phase 8: User Story 6 - Progress tracking across the whole program (Priority: P3)

**Goal**: Recurring violation patterns are tracked and referenced across slices; a trainee's overall progress is reportable on demand and survives a session restart.

**Independent Test**: Repeat the same violation in two different slices; confirm the second occurrence's feedback references the first. Per `quickstart.md` Scenarios 8 and 9.

### Implementation for User Story 6

- [X] T031 [US6] Author the violation-history matching instruction in `.claude/skills/flutter-coach/SKILL.md`: on every evaluation, compare cited violations against `progress-record.md`'s Recurring Violation Patterns by pattern name; append a new entry or increment an existing one (depends on T028, T006)
- [X] T032 [US6] Author the recurrence-callback instruction in `.claude/skills/flutter-coach/SKILL.md`: when a submission repeats an already-flagged pattern, explicitly reference the earlier occurrence in the feedback (depends on T031)
- [X] T033 [US6] Author the progress-summary-on-request instruction in `.claude/skills/flutter-coach/SKILL.md`: when asked for current status, report `current_unlocked_slice`, per-slice status, and recurring patterns directly from `progress-record.md` (depends on T006)
- [X] T034 [US6] Run `quickstart.md` Scenarios 8 and 9 as a dry run, including a full session restart; fix any gaps found

**Checkpoint**: All 6 user stories independently functional; the full spec is covered.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Consistency and final validation across everything produced above.

- [X] T035 [P] Finalize `.claude/skills/flutter-coach/SKILL.md` YAML frontmatter (`name`, `description`, `argument-hint`, `user-invocable: true`) to match the convention used by `.claude/skills/speckit-plan/SKILL.md`
- [X] T036 [P] Proofread all 6 files in `specs/002-flutter-integration-coaching/briefs/` and `SKILL.md` for terminology consistency (slice names, entity names) against `data-model.md`
- [X] T037 [P] Cross-link `briefs/*.md` from `SKILL.md` and vice versa so the skill can always locate the right file for `current_unlocked_slice`
- [X] T038 Run the full `quickstart.md` (all 9 scenarios) end-to-end as a final dry run before a real trainee starts

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - US1, US2, US3 (all P1) form the MVP scope and can proceed in parallel if staffed, or sequentially in the order listed
  - US4, US5 (P2) and US6 (P3) each deepen a specific behavior already present in skeletal form from Phase 2/3
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational — no dependency on other stories; it is the first slice's content plus the basic evaluate/iterate loop
- **User Story 2 (P1)**: Can start after Foundational — independent of US1's *evaluation loop* work, but its consistency check (T019) does read `briefs/auth.md` produced by US1
- **User Story 3 (P1)**: Can start after Foundational — deepens the no-code rule already sketched in Phase 2; independent of US2's brief content
- **User Story 4 (P2)**: Can start after Foundational — deepens gating, independent of US2/US3 content
- **User Story 5 (P2)**: Can start after US1 (builds directly on the evaluate loop authored in T011)
- **User Story 6 (P3)**: Can start after US5 (needs the violation-citation shape from T028 to match patterns against)

### Within Each User Story

- Brief content before skill behavior that references it
- Skill behavior additions before their quickstart dry-run verification
- Story complete (dry run passes) before moving to the next priority

### Parallel Opportunities

- T002 and T003 (Setup) can run in parallel with T001
- T005 and T006 (Foundational) can run in parallel with each other
- All 5 remaining brief-authoring tasks in US2 (T014–T018) can run in parallel with each other (different files)
- T035, T036, T037 (Polish) can run in parallel

---

## Parallel Example: User Story 2

```bash
# Launch all remaining brief-authoring tasks for User Story 2 together (different files):
Task: "Author specs/002-flutter-integration-coaching/briefs/profiles-social-graph.md per contracts/task-brief-template.md"
Task: "Author specs/002-flutter-integration-coaching/briefs/posts-feed.md per contracts/task-brief-template.md"
Task: "Author specs/002-flutter-integration-coaching/briefs/engagement.md per contracts/task-brief-template.md"
Task: "Author specs/002-flutter-integration-coaching/briefs/notifications.md per contracts/task-brief-template.md"
Task: "Author specs/002-flutter-integration-coaching/briefs/search.md per contracts/task-brief-template.md"
```

---

## Implementation Strategy

### MVP First (User Stories 1–3 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1 (Auth slice + basic loop)
4. Complete Phase 4: User Story 2 (remaining 5 briefs)
5. Complete Phase 5: User Story 3 (hardened no-code contract)
6. **STOP and VALIDATE**: run `quickstart.md` Scenarios 1–3 end to end
7. A real trainee can now start the program with all 3 P1 guarantees in place

### Incremental Delivery

1. Complete Setup + Foundational → skill skeleton ready
2. Add US1 → dry-run validate → MVP loop proven on one slice
3. Add US2 → dry-run validate → full curriculum content ready
4. Add US3 → dry-run validate → no-code contract hardened (P1 scope complete)
5. Add US4 → dry-run validate → gating enforced
6. Add US5 → dry-run validate → rubric rigor enforced
7. Add US6 → dry-run validate → cross-session progress tracking complete

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Every "implementation" task in this feature edits either a brief file (content authoring) or `.claude/skills/flutter-coach/SKILL.md` (behavior authoring) — there is no compiled code to build
- Verify via the relevant `quickstart.md` scenario(s) before moving to the next phase
- Avoid: vague tasks, editing `SKILL.md` for two unrelated stories in the same task, cross-story dependencies that break independence
