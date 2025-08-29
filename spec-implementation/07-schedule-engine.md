# 07-schedule-engine.md

Goal
Compute next due time, adherence flags, and deviations.

Inputs
- Last valid shot timestamp
- Interval hours (default 12)
- Vacation plan (optional override schedule)

Outputs
- `NextDueUtc`
- `OnTimeFlag` (on-time/early/late) and `DeviationMinutes`

Rules
- On-time: within ±15 minutes of scheduled slot
- Warn if prior was >15m off
- Respect minimum interval (11h default warn; 12h default target)
