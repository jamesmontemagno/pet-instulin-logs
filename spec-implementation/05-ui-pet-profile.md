# 05-ui-pet-profile.md

Screens
- Pet List (multi-pet)
- Pet Profile (details): name, photo, default units, interval, feeding, vet, emergency contacts, share code

Contracts
- ViewModel exposes: `Pet`, `Save()`, `GenerateShareCode()`
- Validation errors: inline; block save on missing name/units

Acceptance
- Owner can add/edit pet; share code visible
- Caretaker can view but not edit
