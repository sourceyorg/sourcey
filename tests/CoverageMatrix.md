### Sourcey Test Coverage Matrix (2025-12-04 13:23)

This document tracks existing vs. added test coverage across core components.

- Status legend:
  - Present = already existed before this effort
  - Added = implemented by this effort
  - Gap = missing and planned for later

1. Aggregates
   - Unit: behavior/state transitions — Gap
   - Unit: versioning/invariants — Gap
   - Integration: aggregate + store E2E flow — Gap

2. Event Store (Entity Framework Core)
   - Integration: append & read ordering, multi-stream paging — Gap
   - Integration: optimistic concurrency violation — Gap
   - Integration: idempotency/duplicate handling — Gap
   - Integration: global paging by offset — Gap

3. Projections
   - Integration: initial load (large dataset) — Present
   - Integration: eventual consistency matched-on-read — Present
   - Integration: incremental update on new events — Gap
   - Integration: reset and rebuild — Gap

4. Serialization (Newtonsoft)
   - Unit: StreamId converter round-trip (value) — Added
   - Unit: StreamId converter nullable/null handling — Added

5. Caching (EventTypeCache)
   - Unit: case-insensitive simple-name resolution — Added
   - Unit: fully-qualified name resolution — Added
   - Unit: ambiguous simple-name throws AmbiguousMatchException (TryGet/ContainsKey) — Added
   - Unit: not found returns false — Added

Planned next adds (integration):
- Event Store: append/read ordering & paging, optimistic concurrency, idempotency, global paging
- Projections: incremental catch-up and reset/rebuild
- Aggregate-store E2E: aggregate save reflected via projection
