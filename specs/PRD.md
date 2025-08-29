# Pet Insulin Logs — Product Requirements Document (PRD)

Last updated: 2025-08-29
Status: Draft (v0.1)
Owner: Product/Eng (shared)

## 1. Objective
Help pet owners and caretakers reliably track and manage insulin injections for diabetic pets, ensuring safe timing, correct dosing, and clear communication across multiple caregivers and pets.

## 2. Key Outcomes / Success Metrics
- Adherence: ≥90% of scheduled injections logged within ±15 minutes.
- Safety: Automated alerts when an injection is late/early by >15 minutes.
- Usability: Complete a typical log entry in ≤15 seconds (P90).
- Coverage: Support multiple pets per user and multiple caretakers per pet.

## 3. Target Users & Personas
- Pet Owner (Primary): Sets up pets, schedules, dosing, food plans, vet and emergency contacts. Full read/write access.
- Caretaker (Secondary): Temporary or ongoing helper. Can view pet info and history, and create insulin/feeding logs; limited edit rights.

## 4. Core Use Cases
1) Owner creates a pet profile with dosing regimen and feeding plan.
2) Caretaker logs an insulin shot for a scheduled time slot and records location/food.
3) Users review history to confirm adherence and spot irregularities.
4) App shows next due time with warnings if the previous shot was off-schedule.
5) Owner enables Vacation Mode to gradually shift schedule (15-minute increments) toward a new target time.

## 5. Scope (v1)
- Multi-user, multi-pet structure with role-based access (Owner vs Caretaker).
- Pet profiles: name, species/breed (optional), birthdate (optional), weight (optional), photo (optional), unique Pet ID (unguessable), default insulin units, recommended schedule (e.g., every 12 hours), feeding guidance (type/amount), vet info, emergency contacts.
- Logging: per event record with timestamp, insulin units, injection site (left/middle/right shoulder/other), whether food was given and details (type/amount/notes), optional blood glucose notes, freeform notes.
- Schedule awareness: display next due time; detect early/late deviations beyond ±15 minutes and show caution messaging.
- Vacation Mode: plan to shift schedule by 15-minute increments toward a new target time; provide step-by-step daily recommendations.
- History: timeline and list view with filters (by pet, date range, caretaker, late/early flags) and simple stats (on-time rate, average delay, total units).
- Offline-first logging (queued sync) if feasible; otherwise, graceful offline note with retry.

Out of scope (v1): prescriptions management, automatic BG meter integrations, push notifications setup details, cloud account management specifics, analytics dashboard.

## 6. Non-Goals (v1)
- Medical advice or dose recommendations beyond owner-configured defaults.
- Real-time telehealth features.

## 7. Functional Requirements

7.1 Accounts & Roles
- Users can be Owners or Caretakers for specific pets.
- Owners: create/read/update pet profiles; invite/remove caretakers; view full history; log injections and feedings.
- Caretakers: view pet profile; view schedule/next dose; create log entries; cannot change pet configuration.
- Owners and caretakers may each have multiple pets.

7.2 Pet Profile
- Fields: unique Pet ID (unguessable), name (required), default insulin units (required), schedule interval hours (default 12), feeding plan (type/brand, amount, notes), injection site rotation notes (optional), vet information (clinic, phone), emergency contacts (name, relation, phone), optional photo.
- The unique Pet ID must be hard to guess (e.g., UUIDv4 or ULID) and surfaced for sharing with caretakers.

7.3 Logging Events
- Required per log: timestamp (auto-suggest now), insulin units, injection site (left/middle/right shoulder/other).
- Optional per log: was food given (yes/no), food type/amount, notes, caretaker identity, blood glucose notes (free text).
- Validation: units must be positive; timestamp cannot be in the far future beyond configurable tolerance (e.g., >2h), warn on past entries older than 48h.

7.4 Schedule & Adherence
- Next due time is computed from last valid shot + interval (default 12h) unless Vacation Mode plan overrides.
- If previous shot time deviates >15 minutes from scheduled time, show a caution badge and guidance.
- “On-time” definition: within ±15 minutes of scheduled slot.

7.5 Vacation Mode (Schedule Shift)
- Owner selects target AM/PM times; app generates a shift plan using 15-minute increments per dose until alignment.
- Show daily recommendation card: “Today PM: 6:15 pm (shift +15m)”.
- Respect safety constraints: never compress interval below safe minimum (still default 12h; owners can override at their own risk with warnings).
- Ability to pause/cancel plan and revert to standard schedule.

7.6 History & Insights
- List and calendar/timeline views per pet.
- Filters: date range, on-time vs early/late, user (owner/caretaker), injection site, with/without food.
- Stats: on-time rate, average deviation, total units over period.

7.7 Data Integrity & Sharing
- Each pet has a shareable invite/link or code to grant caretaker access; revocation supported.
- Logs are immutable once created; edits create audit trails or are constrained to a grace period (e.g., 15 minutes) with reason required.

## 8. Non-Functional Requirements
- Security & Privacy: store sensitive contact info securely; keep Pet IDs unguessable; least-privilege access per role.
- Reliability: core logging available offline or with degraded mode; sync conflicts resolved last-writer-wins with visible markers.
- Performance: main screens open <1s P90; log creation <15s P90.
- Accessibility: support large text, VoiceOver/TalkBack; color contrast ≥ AA.
- Internationalization: support 12/24h time formats; units field numeric with locale-appropriate formatting.

## 9. Data Model (high-level, implementation-agnostic)
- User: id, name, email (optional), role per pet (owner/caretaker)
- Pet: petId (unguessable), ownerId, name, defaultUnits, intervalHours (default 12), feedingPlan { type, amount, notes }, vet { clinic, phone }, emergencyContacts [{ name, relation, phone }], photoUrl (optional)
- Membership: userId, petId, role
- LogEntry: id, petId, userId, timestamp, units, injectionSite [left|middle|right|other], foodGiven { yesNo, type?, amount? }, notes?, bgNotes?, onTimeFlag [on-time|early|late], deviationMinutes
- VacationPlan: petId, targetTimes { am, pm }, stepMinutes (15), active [bool], generatedSchedule [{ dateTime, note }]

## 10. Key Flows (high-level)
- Onboarding: Owner creates account → adds pet → sets units and 12h schedule → adds vet/emergency → invites caretaker.
- Logging: Select pet → tap “Log shot” → confirm units/time/site → optionally add food → save → next due updates.
- Vacation Mode: Owner sets target times → app proposes steps → owner activates → next due follows plan until complete.

## 11. Edge Cases & Safety Considerations
- Missed dose by >2h: show prominent warning; suggest contacting vet; do not auto-reschedule aggressively.
- Early dose attempt <11h interval: show blocking warning unless owner confirms override with reason.
- Time zone changes/Daylight Saving: schedules should use local time with clear transitions; show banner after TZ change.
- Multiple caretakers log same slot: duplicate detection (same pet within 30m window) prompts review/merge.

## 12. Acceptance Criteria (v1)
- Owner can create/edit pet with unguessable ID; share access with a caretaker.
- Caretaker can log insulin with time, units, food yes/no details, and injection site.
- History view shows all logs with on-time/early/late and deviation minutes.
- Next due time is visible and updates after each log; caution displayed if previous was >15m off.
- Vacation Mode proposes 15-minute increment steps to reach a target time and updates next due accordingly.

## 13. Open Questions
- Authentication approach (local-only vs cloud sync)?
- Push notifications for reminders (platform-specific permissions and timing windows)?
- Audit model: immutable logs vs brief edit window—what’s the preferred policy?
- Offline behavior scope for v1 vs v1.1?

## 14. Future (v1.x+)
- Optional BG meter integrations, CSV export, medication inventory tracking, richer analytics, per-site rotation reminders, widgets/complications.
