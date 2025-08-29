# 06-ui-logging.md

Screens
- Log Shot: time (default now), units (default pet), injection site (Left/Middle/Right/Other), food given (Yes/No + details), notes

Logic
- Submit validates and appends log
- Compute on-time/early/late vs schedule
- Duplicate detection: same pet within 30m → warn

Acceptance
- Both roles can log; success updates next due
- Late/early warning banner if deviation >15m
