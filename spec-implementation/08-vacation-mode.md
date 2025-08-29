# 08-vacation-mode.md

Scope
- Shift schedule to target times using 15-minute increments.

Flows
1) Owner selects target AM/PM times → generate plan
2) Daily recommendation card shows next shift step
3) Pause/cancel plan → revert to standard schedule

Contracts
- `VacationPlan`: `TargetTimes`, `StepMinutes=15`, `Active`, `Generated[]`

Acceptance
- Plan reflects stepwise shifts without compressing intervals below safety threshold
- Next due follows plan when active
