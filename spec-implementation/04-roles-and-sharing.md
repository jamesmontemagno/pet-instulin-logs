# 04-roles-and-sharing.md

Scope
- Owner vs Caretaker roles
- Multi-pet membership
- Sharing via invite code/link; revocation

Flows
1) Owner shares pet access → generates code (PetId + token) → Caretaker joins → membership created (role=Caretaker)
2) Owner revokes caretaker → membership removed

Contracts
- `Membership`: `UserId`, `PetId`, `Role`
- `ShareToken`: unguessable token, expiry, scope: specific `PetId`

Acceptance
- Owner can produce a code for a pet and later revoke caretaker access
- Caretaker sees pet read-only config and can create logs
