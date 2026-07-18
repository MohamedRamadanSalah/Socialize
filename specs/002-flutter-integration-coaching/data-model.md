# Phase 1 Data Model: Flutter Integration Coaching Program

Four entities, all defined in `spec.md` Key Entities. This document gives each one concrete fields, relationships, and validation rules. None of these are database tables — "persisted" here means written to the Markdown files under `specs/002-flutter-integration-coaching/` and `tasks.md`, per the research-phase format decision.

## Vertical Slice Task Brief

One per backend feature area; lives as a section of `tasks.md`, structured per `contracts/task-brief-template.md`. Not mutable at runtime — authored once in Phase 2, revisited only if the backend contract for that slice changes (spec Edge Cases).

| Field | Type | Notes |
|---|---|---|
| `slice_id` | slug | One of: `auth`, `profiles-social-graph`, `posts-feed`, `engagement`, `notifications`, `search` |
| `feature_name` | string | Human-readable name, matches the backend feature area in `001-social-media-backend/spec.md` |
| `sequence_position` | integer 1–6 | Fixed order per FR-004: auth=1, profiles-social-graph=2, posts-feed=3, engagement=4, notifications=5, search=6 |
| `prerequisite_slice_id` | slug or null | The slice that must be passed before this one unlocks; null only for `auth` |
| `backend_endpoints` | list of `{method, path, contract_ref}` | Points into `backend/API.md` / `001-social-media-backend/contracts/` — this brief never restates the contract, only references it |
| `architecture_layers` | list of `{layer: domain|data|presentation, responsibility}` | Per FR-002/FR-005; describes responsibility, not concrete class names |
| `riverpod_providers` | list of `{purpose, provider_type}` | e.g. `{purpose: "current auth session", provider_type: "NotifierProvider"}` — type, not class name |
| `ui_flows` | list of strings | Screens/flows the slice requires, in plain language |
| `acceptance_criteria` | list of strings | Directly testable statements, mirrors the corresponding spec-001 acceptance scenarios |
| `definition_of_done` | list of checklist items | Used verbatim by the coach for partial-credit evaluation (spec Edge Cases) |

**Validation rules**: `sequence_position` values are unique across all 6 briefs and contiguous 1–6. Every `backend_endpoints` entry must resolve to a real endpoint documented in `backend/API.md`. `prerequisite_slice_id` forms a single linear chain (no branching, no cycles) matching FR-004's fixed order.

## Submission

An in-conversation event, not a standalone stored file — its effect is a new row in the Evaluation's history and, if it changes the recorded evaluation status, an update to `progress-record.md`.

| Field | Type | Notes |
|---|---|---|
| `slice_id` | slug | The slice being submitted |
| `signaled_at` | conversational turn reference | Not a wall-clock timestamp requirement — just "this turn was recognized as a submission" per the Research §1 signal convention |
| `trainee_explanation` | string | The trainee's own account of key architectural decisions (FR-012); required before an overall pass can be issued |

**Validation rules**: A Submission MUST reference a `slice_id` that is currently unlocked in `progress-record.md` (i.e., its `prerequisite_slice_id` is already `passed`) OR the trainee has explicitly invoked a recorded skip (FR-013/FR-014). A Submission without a recognized explicit signal (Research §1) is not a Submission — it's ordinary conversation and MUST NOT produce an Evaluation.

## Evaluation

The coach's structured verdict on one Submission; its shape is fixed by `contracts/evaluation-rubric.md`. The latest Evaluation per slice (plus a count of prior attempts) is what `progress-record.md` retains — full evaluation transcripts are not archived verbatim, only their outcome and cited violations (so the file stays small and diffable).

| Field | Type | Notes |
|---|---|---|
| `slice_id` | slug | The slice evaluated |
| `dimension_verdicts` | `{functional_correctness, architecture_compliance, riverpod_usage, code_quality} → pass \| needs_revision` | Per FR-009/FR-010; all four required on every evaluation |
| `cited_violations` | list of `{dimension, description}` | Required whenever the matching dimension is `needs_revision`; empty list when a dimension passes |
| `overall_verdict` | `pass \| needs_revision` | `pass` only if all four dimensions are `pass` AND the trainee has supplied a satisfactory `trainee_explanation` (FR-012) |
| `guiding_next_steps` | list of questions/pointers | Never contains code (FR-006, FR-010) |

**Validation rules**: `overall_verdict = pass` requires all four `dimension_verdicts` to be `pass` (Story 5, Scenario 3) — an automated-checks-only pass with an `architecture_compliance` or `riverpod_usage` violation MUST still yield `overall_verdict = needs_revision` (FR-011). Every `needs_revision` dimension MUST have at least one entry in `cited_violations`.

## Progress Record

The single persisted file, `progress-record.md`, one per engagement (one trainee). Read at the start of every session; updated immediately after every Evaluation and after every recorded skip.

| Field | Type | Notes |
|---|---|---|
| `current_unlocked_slice` | slug | The next slice the trainee is allowed to receive a brief for |
| `slice_status[]` | per slice: `not_started \| in_revision \| passed` | One entry per of the 6 slices |
| `skip_deviations[]` | list of `{from_slice, to_slice, noted_at_turn}` | Recorded whenever FR-014 triggers |
| `violation_history[]` | list of `{pattern_description, first_flagged_slice, recurrence_count}` | Grown by FR-015/FR-016; a recurrence increments `recurrence_count` and triggers the "reference the earlier occurrence" behavior |

**Validation rules**: Exactly one `slice_status` entry per slice, always 6 total. `current_unlocked_slice` MUST equal the earliest slice (by `sequence_position`) whose status is not `passed`, unless a `skip_deviations` entry explicitly moved it forward. `violation_history` entries are keyed by a stable `pattern_description` so the same mistake recurring across slices increments one entry rather than creating duplicates.

## Relationships

```text
Vertical Slice Task Brief (1) ──unlocks──> Vertical Slice Task Brief (next, via prerequisite_slice_id)
Vertical Slice Task Brief (1) ──receives──> Submission (0..N, one per attempt)
Submission (1) ──produces──> Evaluation (1)
Evaluation (N, across all slices) ──rolls up into──> Progress Record (1, the whole engagement)
```
