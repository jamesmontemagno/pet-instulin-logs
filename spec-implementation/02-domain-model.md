# 02-domain-model.md

Entities (high-level, C#-friendly)
- User: `UserId`, `Name?`, `Email?`
- Pet: `PetId`, `OwnerId`, `Name`, `DefaultUnits`, `IntervalHours`, `FeedingPlan`, `VetInfo`, `EmergencyContacts[]`, `PhotoUri?`
- Membership: `UserId`, `PetId`, `Role` (Owner|Caretaker)
- LogEntry: `LogId`, `PetId`, `UserId`, `TimestampUtc`, `Units`, `InjectionSite` (Left|Middle|Right|Other), `FoodGiven` ({YesNo, Type?, Amount?}), `Notes?`, `BgNotes?`, `OnTimeFlag`, `DeviationMinutes`
- VacationPlan: `PetId`, `TargetTimes` ({AM, PM}), `StepMinutes=15`, `Active`, `Schedule[]`

Validation
- `Units > 0`
- `TimestampUtc` not > now + 2h; warn if < now - 48h

Mapping
- Persistence DTOs mirror entities; add version field for migration.
