# Contract: Vertical Slice Task Brief Template

Every brief in `tasks.md` (Phase 2) MUST follow this exact section structure, so a trainee can always find the same information in the same place across all 6 slices (spec Story 2, FR-002).

```markdown
## Slice N: <Feature Name>

**Prerequisite**: <previous slice name, or "None — start here">
**Backend contract**: <links/references into backend/API.md for every endpoint this slice touches>

### Goal
<1–3 sentences: what this slice lets a user of the Flutter app do>

### Backend Endpoints Involved
- `<METHOD> <path>` — <one-line purpose> (see backend/API.md § ...)
- ...

### Required Architecture Layers
- **Domain**: <entities / repository interfaces / use cases this slice needs, described by responsibility>
- **Data**: <repository implementation(s), remote data source(s), DTO/mapper responsibility>
- **Presentation**: <Riverpod provider(s) and their purpose, screens/widgets needed>

### Required Riverpod Providers
- <purpose> — <provider type, e.g. NotifierProvider / FutureProvider / StreamProvider> (no class names prescribed)
- ...

### UI Flows
- <screen/flow 1>
- <screen/flow 2>

### Acceptance Criteria
1. Given ..., when ..., then ... (mirrors the matching acceptance scenario in `001-social-media-backend/spec.md`)
2. ...

### Definition of Done
- [ ] <checklist item>
- [ ] <checklist item>
- ...

### Submitting This Slice
When you believe this slice is complete, tell the coach explicitly (e.g. "I'm done with <slice>, please evaluate" or "submit for evaluation") — see `contracts/evaluation-rubric.md` for what happens next. Casual questions with code attached are not graded; only an explicit submission is.
```

**Rules for whoever authors `tasks.md`**:
- Every section listed above is mandatory for every slice — no slice may omit a section.
- "Required Architecture Layers" and "Required Riverpod Providers" describe *responsibility and type*, never concrete class/file names (FR-005) — the trainee designs the specifics.
- "Backend Endpoints Involved" must reference real, existing endpoints in `backend/API.md`; never invent an endpoint.
- Slice order in `tasks.md` must match `sequence_position` in `data-model.md` (Auth → Profiles/Social Graph → Posts/Feed → Engagement → Notifications → Search).
