# 01-foundations.md

Goal: Establish app foundations for modular growth.

Scope
- Project structure confirmation (MAUI)
- Core domain entities and IDs
- Storage approach (local-first with future cloud sync)
- Error/logging/telemetry hooks

Contracts
- IDs: use unguessable IDs for pets (UUIDv4/ULID). Example: `PetId: string`.
- Time: store timestamps as ISO 8601 in UTC; render in local time.
- Interval defaults: `intervalHours = 12`; adherence window ±15m.

Acceptance
- Build runs on iOS/Android/MacCatalyst.
- A data layer interface exists with in-memory/local implementation stub.
- Utilities for time and ID generation available.

Sequencing
- Create domain project/files; add DI registrations in `MauiProgram.cs`.
