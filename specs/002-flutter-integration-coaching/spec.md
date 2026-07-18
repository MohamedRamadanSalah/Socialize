# Feature Specification: Flutter Integration Coaching Program

**Feature Branch**: `002-flutter-integration-coaching`

**Created**: 2026-07-18

**Status**: Draft

**Input**: User description: "we need to write very detailed tasks to make the flutter dev to intigrate with this backend in best practices in steps and in tasks per feature he must use vertical slices and riverpod and clean arch in its work and when he write code must the ai claude coaching him and evalute him not write any code he must be a coach alone the tasks must be very detailed becuse he know what he must do in details"

## Clarifications

### Session 2026-07-18

- Q: Should the coaching contract (no-code rule, rubric, gating) be delivered as a persisted, reusable project artifact that any future session automatically loads and enforces, or is documentation alone (manually re-briefed each session) sufficient? → A: A persisted, reusable project artifact (e.g., a project skill/instructions file) that any future session automatically loads and follows.
- Q: Should per-trainee progress/evaluation history persist across separate sessions, or is it acceptable for it to live only within one continuous conversation? → A: Persist in a file the coach reads at the start of a session and updates after each evaluation, so a new session resumes exactly where the last left off.
- Q: Does a graded evaluation require an explicit signal from the trainee, or should the coach auto-evaluate any code it sees mid-conversation? → A: Evaluation only happens on an explicit trainee signal (e.g., "submit for evaluation"); casual code-sharing during Q&A is never auto-graded.

## User Scenarios & Testing *(mandatory)*

This feature defines a **coaching program**, layered on top of the existing Socialize backend (see `001-social-media-backend`), that turns backend integration into a structured Flutter learning path. Two actors participate:

- **Trainee** — the Flutter developer building a mobile client against the Socialize backend.
- **AI Coach** — an AI mentor/evaluator (Claude) that guides and grades the trainee's work but never authors implementation code on the trainee's behalf.

### User Story 1 - Guided first vertical slice: Authentication (Priority: P1)

A trainee starting the program receives a single, detailed task brief for the Authentication & Session vertical slice — the first and most foundational backend feature. The brief tells them exactly what to build (register, login, silent refresh, logout) using Clean Architecture layering and Riverpod for state, without telling them the exact code to write. They implement it, submit it for review, and the AI coach evaluates it against a rubric and gives feedback until it passes.

**Why this priority**: Nothing else in the program can proceed without establishing the coaching workflow itself (brief → build → submit → evaluate → iterate) on the single feature every later slice depends on.

**Independent Test**: Give a trainee only the Authentication task brief; confirm they can build a working, independently-runnable register/login/refresh/logout flow against the real backend and receive a pass verdict from the coach, without the coach ever supplying code.

**Acceptance Scenarios**:

1. **Given** a trainee starting the program, **When** they receive the Authentication vertical-slice brief, **Then** it specifies the backend endpoints to integrate, the required domain/data/presentation layers, the required Riverpod providers, explicit acceptance criteria, and a definition-of-done checklist.
2. **Given** a trainee working on the brief, **When** they ask the AI coach a question about how to proceed, **Then** the coach responds with clarifying questions, hints, or references to the brief/backend contract — never with code the trainee can copy in.
3. **Given** a completed Authentication implementation, **When** the trainee submits it for evaluation, **Then** the coach returns a structured verdict covering functional correctness, Clean Architecture compliance, and Riverpod usage, with specific cited issues if any.
4. **Given** a failing evaluation, **When** the trainee revises and resubmits, **Then** the coach re-evaluates the same rubric and the trainee can eventually reach a passing verdict without the coach having written any of the fix.

---

### User Story 2 - A detailed, self-sufficient task brief per backend feature (Priority: P1)

Every backend feature area (Authentication & Sessions, Profiles & Social Graph, Posts & Feed, Engagement, Notifications & Real-time, Search) has its own standalone, detailed task brief, so a trainee always knows precisely what to build next without needing to ask the coach "what do I do."

**Why this priority**: The user's core ask is detailed, per-feature tasks. Without this, the coaching interaction has nothing concrete to guide or evaluate against, and trainees would be blocked waiting on the coach to describe scope.

**Independent Test**: Hand any single feature's task brief to a trainee with no other context beyond the backend integration guide; confirm they can identify the endpoints involved, the layers to build, the Riverpod providers needed, and how their work will be judged, entirely from the brief.

**Acceptance Scenarios**:

1. **Given** the set of backend feature areas, **When** task briefs are produced, **Then** there is exactly one brief per feature area and every brief follows the same structure (goal, backend contract references, required architecture layers, required Riverpod providers, UI flows, acceptance criteria, definition-of-done checklist).
2. **Given** a task brief, **When** a trainee reads it before writing any code, **Then** they can state what "done" looks like without further clarification from the coach.
3. **Given** the full set of briefs, **When** they are sequenced, **Then** the order matches the backend's own priority order so each delivered slice is an independently demonstrable increment.

---

### User Story 3 - Coach never writes code; only questions, hints, and verdicts (Priority: P1)

Whenever the trainee asks for help or submits code, the AI coach engages only through Socratic questioning, architectural critique, and rubric-based verdicts. It never produces a code snippet, diff, function body, or file the trainee could paste directly into their project — regardless of how directly the trainee asks for one.

**Why this priority**: This is the non-negotiable behavioral contract the user is asking for; without it, the program collapses into the coach doing the trainee's job.

**Independent Test**: Have a trainee directly ask the coach to "just write the code," in several phrasings across several feature slices; confirm every response declines to author code and instead redirects with questions, hints, or pointers to the brief/spec/backend contract.

**Acceptance Scenarios**:

1. **Given** any trainee request, however phrased, **When** it amounts to asking for implementation code, **Then** the coach declines to produce code and instead asks a guiding question or points to relevant documentation/architecture concepts.
2. **Given** a trainee stuck on the same issue across multiple exchanges, **When** they remain stuck, **Then** the coach may increase hint specificity (e.g., naming the exact concept or pattern to research) while still stopping short of supplying the code itself.
3. **Given** a trainee submission, **When** the coach evaluates it, **Then** the evaluation output contains cited issues and guiding questions, never a rewritten version of the trainee's code.

---

### User Story 4 - Gated progression across vertical slices (Priority: P2)

A trainee cannot start the next vertical slice's task brief until their current slice has received a passing evaluation (or they explicitly and knowingly choose to skip ahead), keeping the learning path in backend-priority order: Authentication → Profiles/Social Graph → Posts/Feed → Engagement → Notifications → Search.

**Why this priority**: Enforces the "in steps" structure the user asked for and prevents trainees from building later features (e.g., a feed) on top of an unvalidated foundation (e.g., broken auth).

**Independent Test**: Attempt to obtain the Posts/Feed brief before Authentication has passed evaluation; confirm the coach declines and redirects to the outstanding Authentication evaluation, unless the trainee explicitly requests to skip.

**Acceptance Scenarios**:

1. **Given** an unpassed current slice, **When** the trainee asks for the next slice's brief, **Then** the coach declines and points back to the outstanding evaluation.
2. **Given** a passing evaluation on the current slice, **When** the trainee asks for the next brief, **Then** the coach provides it.
3. **Given** a trainee who explicitly requests to skip ahead, **When** they do so, **Then** the coach proceeds but records the deviation from the standard order.

---

### User Story 5 - Architecture and pattern compliance evaluation (Priority: P2)

For every submission, the coach explicitly evaluates Clean Architecture layering (no framework code in the domain layer, repository-interface/implementation separation, correct dependency direction) and Riverpod conventions (appropriate provider types, no business logic embedded in widgets, correct async/state handling), citing specific violations rather than giving a generic pass/fail.

**Why this priority**: This is what makes the evaluation "best practices" rather than merely "does it run" — central to the user's stated goal, but it depends on the rubric and workflow established in Stories 1–3.

**Independent Test**: Submit a piece of work that runs correctly but puts business logic inside a widget and skips a repository interface; confirm the coach marks it as failing and cites the specific architecture/Riverpod rules broken.

**Acceptance Scenarios**:

1. **Given** a submission that compiles and functions correctly but violates a named Clean Architecture rule, **When** it is evaluated, **Then** the coach marks it as failing on that dimension and cites the specific violation.
2. **Given** a submission that violates a named Riverpod convention (e.g., business logic in a widget), **When** it is evaluated, **Then** the coach marks it as failing on that dimension and cites the specific violation.
3. **Given** a submission that satisfies all rubric dimensions, **When** it is evaluated, **Then** the coach marks it as an overall pass.

---

### User Story 6 - Progress tracking across the whole program (Priority: P3)

The coach keeps track of which slices a trainee has passed, which are in revision, and which mistakes recur across slices, so later coaching sessions can reference earlier feedback instead of repeating the same guidance from scratch.

**Why this priority**: Improves coaching quality over the length of the program but the program is functional slice-by-slice without it.

**Independent Test**: After a trainee repeats the same Riverpod provider-scoping mistake in two different slices, confirm the coach's feedback on the second occurrence references the earlier, already-flagged pattern.

**Acceptance Scenarios**:

1. **Given** a trainee partway through the program, **When** progress is reviewed, **Then** it shows which slices passed, which are in revision, and any recurring violation patterns.
2. **Given** a violation pattern already flagged in an earlier slice, **When** it recurs in a later submission, **Then** the coach's feedback references the earlier occurrence.

---

### Edge Cases

- A trainee directly asks the coach to "just write it" or to "give an example implementation" — the coach declines and reframes with a guiding question, in every phrasing.
- A trainee is stuck on the same rubric violation across several exchanges — the coach escalates hint specificity without ever supplying code.
- A submission passes automated compilation/tests but violates a named architecture or Riverpod rule — it is still marked failing.
- A trainee cannot explain the reasoning behind their own architectural choices when asked — the coach withholds a passing verdict pending that explanation.
- A trainee tries to obtain a later slice's brief without a passing evaluation on the current one — the coach declines unless the trainee explicitly opts to skip.
- A submission only partially implements a slice's definition-of-done checklist — it is evaluated item-by-item; missing items are cited rather than an unexplained overall fail.
- The backend contract for a slice changes after that slice's brief was issued — the brief and rubric for that slice are revisited before further evaluation against it.

## Requirements *(mandatory)*

### Functional Requirements

**Task Briefs**
- **FR-001**: The program MUST provide exactly one detailed task brief per backend feature vertical slice: Authentication & Sessions, Profiles & Social Graph, Posts & Feed, Engagement (likes/comments), Notifications & Real-time, and Search — mapped 1:1 to the feature areas defined in the backend spec (`001-social-media-backend`).
- **FR-002**: Each task brief MUST specify, in writing and before any code is written: the backend endpoint(s)/contracts the slice integrates with, the required Clean Architecture layers and each layer's responsibility for that slice, the required Riverpod provider types and their scope, the UI screens/flows needed, explicit acceptance criteria, and a definition-of-done checklist.
- **FR-003**: Task briefs MUST instruct the trainee to implement each feature as an independent vertical slice — its own domain, data, and presentation code path — rather than by extending shared/global layers first.
- **FR-004**: Task briefs MUST be sequenced to match the backend's own priority order (Authentication first; then Profiles/Social Graph and Posts/Feed; then Engagement and Notifications; then Search last), so each completed slice is an independently demonstrable increment.
- **FR-005**: The program MUST define, per vertical slice, the minimum expected architecture artifacts (e.g., domain entity/model, repository interface and implementation, use case/interactor, Riverpod provider(s), presentation state, screens/widgets) without prescribing exact class or file names, leaving concrete design to the trainee.

**Coaching Behavior**
- **FR-006**: The AI coach MUST NOT author, generate, or supply implementation code, code diffs, or complete function/class bodies to the trainee at any point, regardless of how the trainee's request is phrased.
- **FR-007**: The AI coach MUST respond to requests for help using Socratic technique — clarifying questions, references to the relevant brief/backend contract/architecture concept, and hints — that lead the trainee to their own answer.
- **FR-008**: When a trainee remains stuck on the same issue across repeated exchanges, the AI coach MUST be permitted to increase hint specificity (e.g., naming the exact pattern, provider type, or concept to research) while still never supplying the code itself.

**Evaluation**
- **FR-009**: The AI coach MUST run a graded evaluation only when the trainee gives an explicit submission signal (e.g., "submit for evaluation" / "I think this slice is done"); code shared incidentally while asking a question MUST NOT be auto-graded. On an explicit submission, the coach MUST evaluate the work against an explicit rubric covering: functional correctness against the backend contract, Clean Architecture compliance (layer separation, dependency direction, no framework leakage into the domain layer), Riverpod usage correctness (appropriate provider types, no business logic in widgets, correct async/state handling), and general code quality.
- **FR-010**: The AI coach MUST produce a structured evaluation result for each submission: a pass/fail (or pass/needs-revision) verdict per rubric dimension, specific cited violations describing what and where, and actionable next-step guidance framed as questions or areas to revisit — without providing the fix itself.
- **FR-011**: If a submission passes automated checks (e.g., it compiles, its tests are green) but violates a named Clean Architecture or Riverpod rule, the AI coach MUST still mark it as failing on that rubric dimension — automated-check success alone MUST NOT be sufficient for an overall pass.
- **FR-012**: Before issuing an overall passing verdict, the AI coach MUST require the trainee to explain, in their own words, the reasoning behind key architectural decisions in the submission.

**Progression & Progress Tracking**
- **FR-013**: The program MUST gate progression: a trainee MUST receive a passing evaluation on the current vertical slice before the next slice's task brief (per the FR-004 order) is provided, unless the trainee explicitly requests to skip ahead.
- **FR-014**: When a trainee explicitly skips a slice without a passing evaluation, the program MUST record this as a deviation from the standard order.
- **FR-015**: The AI coach MUST track, per trainee, which slices have passed, which are currently in revision, and any violation patterns that have recurred across more than one slice.
- **FR-016**: When a previously flagged violation pattern recurs in a later submission, the AI coach's feedback MUST reference the earlier occurrence.
- **FR-017**: The Progress Record MUST persist to a file the AI coach reads at the start of a session and updates immediately after each evaluation, so that progress, gating state, and recurring violation patterns survive a session ending and are available at the start of the next one.

**Delivery & Enforcement**
- **FR-018**: The coaching contract — task briefs, the no-code rule, the evaluation rubric, and slice gating — MUST be encoded in a persisted, reusable project artifact (e.g., a project skill/instructions file committed to the repository) that any future session automatically loads and follows, rather than depending on ad hoc re-briefing of the assistant each session.

### Key Entities *(include if feature involves data)*

- **Vertical Slice Task Brief**: One per backend feature area. Attributes: feature name, priority/sequence position, backend endpoints/contracts covered, required Clean Architecture layers and responsibilities, required Riverpod provider types, UI flows, acceptance criteria, definition-of-done checklist, prerequisite slice(s).
- **Submission**: A trainee's explicit submission of completed work for one task brief at a point in time, distinct from code shared incidentally during Q&A. Attributes: referenced slice, submitted-at time, trainee's own explanation of key design decisions.
- **Evaluation**: The coach's structured verdict on a submission. Attributes: per-dimension pass/fail (functional correctness, architecture compliance, Riverpod usage, code quality), cited violations, guiding questions/next steps, overall verdict, timestamp.
- **Progress Record**: Cumulative per-trainee state, persisted to a file read/updated by the coach across sessions. Attributes: slices passed, slices in revision, recorded skip deviations, recurring violation patterns, currently unlocked slice.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 6 backend feature areas have a corresponding, self-sufficient task brief detailed enough that a trainee can begin work without asking the coach what to build.
- **SC-002**: 100% of coaching responses across a training engagement contain no authored implementation code (snippets, diffs, or complete implementations), verified by reviewing coach outputs.
- **SC-003**: In at least 95% of tracked training sessions, a trainee does not receive the next slice's brief before their current slice reaches a passing evaluation (excluding explicit, recorded skip requests).
- **SC-004**: 100% of submissions that pass automated compilation/tests but violate a named Clean Architecture or Riverpod rule are still correctly marked as failing.
- **SC-005**: A trainee who completes all 6 vertical slices ends the program with a working Flutter client covering every backend feature area, with zero implementation code contributed by the coach.
- **SC-006**: When a violation pattern is flagged a second time for the same trainee, the very next relevant evaluation references the earlier occurrence.

## Assumptions

- The trainee already has a working Flutter development environment; local environment/toolchain setup is out of scope for this coaching program.
- The Socialize backend defined in `001-social-media-backend` is the fixed integration target; its endpoints and contracts are the source of truth for "functional correctness."
- "Vertical slice" means building the domain, data, and presentation layers for one feature end-to-end before moving to the next feature, rather than building all domain layers first, then all data layers, and so on.
- Riverpod is the sole state-management approach accepted in evaluation; no other state-management library satisfies the Riverpod-usage rubric dimension.
- Clean Architecture compliance means, at minimum: the domain layer has no dependency on Flutter, Riverpod, or HTTP packages; the data layer implements domain-defined repository interfaces; the presentation layer depends on the domain via providers rather than talking to data sources directly.
- One coaching engagement corresponds to one trainee; their Progress Record persists in a file across sessions so a new session resumes their gating state and history. Multi-trainee cohort dashboards or leaderboards are out of scope for this version.
- The standard slice order follows the backend's own priority order (Authentication → Profiles/Social Graph & Posts/Feed → Engagement & Notifications → Search) unless the trainee explicitly requests to skip ahead.
- This specification defines what the task briefs and coaching/evaluation behavior must guarantee; authoring the actual detailed brief content for each slice is the subject of the subsequent planning and task-generation phases.
