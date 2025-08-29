# 03-storage-and-sync.md

Approach
- v1: Local storage (preferences + local DB, e.g., SQLite via EF Core or plain SQLite)
- v1.1+: Optional cloud sync (TBD)

Interfaces
- `IPetRepository`: CRUD for Pet, Membership, sharing tokens
- `ILogRepository`: append-only LogEntry, query by pet/date, stats helpers
- `IVacationPlanRepository`: get/set plan per pet

Decisions
- Logs immutable; edit within 15m grace with audit note
- Simple conflict policy: last-writer-wins; mark conflicts in UI

Acceptance
- Can persist and query pets and logs locally
- Basic migrations strategy defined (versioned schema in a single place)
