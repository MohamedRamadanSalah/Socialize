# Phase 0 Research: Flutter Integration Coaching Program

## 1. Submission-signal convention

**Decision**: The coach recognizes an explicit submission only when the trainee's message contains a clear, unambiguous signal that they are done with a specific slice and want it graded — e.g. "submit for evaluation", "I'm done with &lt;slice&gt;, please evaluate", "grade this", "review my &lt;slice&gt; implementation now". No slash command or formal ceremony is required.

**Rationale**: The spec (FR-009, clarified 2026-07-18) requires an explicit signal but the coaching interaction is conversational by nature (Story 3), and this repo's `/speckit-*` slash-command convention is for spec-kit workflow steps, not for a free-form mentoring conversation. Requiring a formal command would fight the coaching tone and add friction the spec doesn't ask for. A short list of recognized signal phrases, checked against the trainee's message, is enough to reliably distinguish "here's a question with some code in it" from "grade this."

**Alternatives considered**:
- A dedicated slash command (e.g. `/submit posts-feed`) — rejected: adds tooling surface the spec doesn't require and breaks the conversational Socratic tone Story 3 establishes.
- Coach infers intent purely from context with no fixed phrase list — rejected: too ambiguous to reliably guarantee FR-009's "never auto-grade incidental code" guarantee; a fixed phrase list makes the boundary auditable.

## 2. Coaching contract delivery mechanism

**Decision**: Encode the contract as a new project skill, `.claude/skills/flutter-coach/SKILL.md`, following this repository's existing skill convention (YAML frontmatter with `name`, `description`, `argument-hint`, `user-invocable: true`), invoked as `/flutter-coach`.

**Rationale**: FR-018 (clarified) requires a persisted, reusable, auto-loaded artifact. This repository already ships all of its process behavior (spec-kit workflow, code review, etc.) as project skills under `.claude/skills/`, so a new skill is the path of least surprise and reuses infrastructure that's already proven to auto-load into a session.

**Alternatives considered**:
- A `CLAUDE.md` addendum (always-on project instructions) — rejected: `CLAUDE.md` is global project guidance; a dedicated skill keeps the coaching contract scoped to when the trainee is actually in a coaching session, and matches how `/speckit-*` already isolates workflow-specific behavior.
- An external tool/script that gates the conversation programmatically — rejected: over-engineered for a conversational coaching contract; nothing here needs code execution, only consistent instructions.

## 3. Progress-record file format

**Decision**: `progress-record.md`, a single Markdown file with a small fixed structure (current unlocked slice, per-slice status table, recorded skip deviations, recurring-violation log) — see `contracts/progress-record.schema.md`.

**Rationale**: FR-017 (clarified) only requires "a file the coach reads/updates," not a specific format. Markdown keeps the file human-readable (the trainee can open and understand their own status) and diff-friendly in git, consistent with every other artifact in this spec-kit-driven repository. A single file matches the "one trainee per engagement" scope (Assumptions) — no multi-file/per-trainee-directory scheme is needed.

**Alternatives considered**:
- JSON — rejected: more precise for programmatic consumption, but nothing here needs to be machine-parsed by anything other than Claude reading Markdown; JSON is worse for a human trainee to read directly and inconsistent with the rest of the repo's docs-as-source-of-truth style.
- No persisted file, rely on conversation memory only — rejected outright by the clarification answer (progress must survive a session ending).

## 4. Flutter-side architectural convention the rubric checks against

**Decision**: Feature-first (vertical-slice) folder layout, one `domain/` (entities, repository interfaces, use cases), `data/` (repository implementations, remote data source, DTOs/mappers), and `presentation/` (Riverpod providers, screens/widgets) per backend feature area; Riverpod using the code-generation style (`@riverpod` annotations via `riverpod_generator`), which is the current idiomatic convention for new Riverpod code.

**Rationale**: This gives the rubric (FR-005, FR-009) concrete, checkable structure without dictating exact class names — matching FR-005's "minimum expected architecture artifacts... without prescribing exact class or file names." It mirrors the same dependency-rule shape the backend itself already uses (`001-social-media-backend/plan.md`: Domain depends on nothing, outer layers depend inward), giving the trainee a consistent mental model across both sides of the integration.

**Alternatives considered**:
- Layer-first folders (`lib/domain/`, `lib/data/`, `lib/presentation/` each containing all features) — rejected: this is exactly what FR-003 tells trainees *not* to do ("rather than extending shared/global layers first"); vertical slices need feature-first folders to be checkable as independent.
- Manual (non-codegen) Riverpod providers (`Provider`, `StateNotifierProvider` written by hand) — considered acceptable as a valid alternative style; the rubric will accept either manual or codegen Riverpod as long as provider *types* and *scoping* are correct (FR-009), but codegen is the recommended default referenced in briefs since it's the current idiomatic default and reduces boilerplate-related mistakes unrelated to the actual learning goal.

## 5. Real-time (Notifications slice) client approach

**Decision**: The Notifications brief tells the trainee to integrate with the backend's SignalR hub using a community SignalR client package for Dart (e.g. `signalr_netcore`), wrapped behind a domain-level `NotificationsRealtimeSource` interface so the concrete transport is swappable and testable — the rubric checks for the interface/abstraction, not the specific package choice.

**Rationale**: The backend (`001-social-media-backend/plan.md`) uses ASP.NET Core SignalR for real-time push (FR-020/FR-021 in that spec). There is no official Dart/Flutter SignalR client, so trainees need a documented, named option rather than discovering this gap mid-slice — but the *architecture* lesson (abstracting the transport behind a domain interface, per Clean Architecture) matters more than the specific package, so the rubric evaluates the abstraction rather than mandating one library.

**Alternatives considered**:
- Mandating a specific package version — rejected: package churn in the Dart ecosystem would make the brief go stale; naming one recommended option while grading the abstraction is more durable.
- Raw WebSocket fallback instead of a SignalR-protocol client — mentioned as an acceptable alternative transport in the brief (SignalR's underlying transport is negotiable), but not the default recommendation, since a protocol-aware client handles reconnection/handshake details the trainee would otherwise have to reimplement, which isn't the point of this slice.
