# Pet Insulin Logs 🐾💉

A simple, friendly app to help pet owners and caretakers track insulin injections and feeding for diabetic pets—on time, safely, and with confidence.

## Why this app? 🌟
Diabetic pets usually need insulin twice a day about 12 hours apart. This app keeps everyone on the same page: what dose, when it was given, whether food was given, and where the shot was administered—plus gentle guidance if timing drifts.

## Key features ✅
- Multi-pet support 🐶🐱
- Owner & Caretaker roles 👑🤝 (owners configure; caretakers log and view)
- Quick logging 🕒💉
  - Timestamp, units, food given 🍽️, injection site 🎯 (left/middle/right shoulder/other), notes
- Schedule awareness ⏰
  - Next due time, on-time window (±15m), caution on early/late shots ⚠️
- Vacation Mode 🌴
  - Shift schedule with 15-minute step recommendations until target time
- History & Insights 📜📈
  - Filters (date range, user, late/early, site, with/without food), simple stats
- Unguessable pet IDs 🔐 for safer sharing
- Accessibility & i18n ♿🌐 (large text, screen readers, 12/24h time)
- Offline-friendly 📴 (planned) with safe conflict handling

## Screens & flows 🧭
- Pet profile: name, default units, schedule, feeding plan, vet & emergency contacts
- Log shot: confirm units/time/site, optional food/notes
- Next due: see upcoming time and drift warnings
- Vacation plan: gradual schedule shifts with daily cards
- History: timeline and stats

## Docs 📚
- Product Requirements: specs/PRD.md
- Implementation Plan: spec-implementation/ (modular steps & acceptance criteria)

## Tech stack 🛠️
- .NET MAUI for cross-platform mobile/desktop (Android, iOS, MacCatalyst)
- Local storage first (SQLite), optional cloud sync later

## Run locally 🚀
- Requirements: .NET SDK 9, MAUI workloads installed (Android/iOS/Mac)
- Project: PetInsulinLogs/PetInsulinLogs.csproj

Optional commands (if you have MAUI set up):

```bash
# restore workloads if needed
# dotnet workload restore

# build
# dotnet build PetInsulinLogs/PetInsulinLogs.csproj

# run (choose a target like android/ios/maccatalyst)
# dotnet build -t:Run -f net9.0-maccatalyst PetInsulinLogs/PetInsulinLogs.csproj
```

Note: Platform setup (SDKs, emulators, signing) may be required—see MAUI docs.

## Contributing 🤗
- Review the PRD and spec-implementation files first
- Tackle milestones in order (Foundations → Profiles/Sharing → Logging/Schedule → History → Vacation → Polish/QA)
- Keep changes small and tied to the acceptance criteria in each spec

## Roadmap 🗺️
- v1: Core logging, schedule awareness, vacation mode, history
- v1.x: Offline sync, exports, richer insights, BG device integrations (exploratory)

---
Questions or ideas? Open an issue or start a discussion! 💬
