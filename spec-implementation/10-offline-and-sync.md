# 10-offline-and-sync.md

Approach
- Graceful offline: allow logging; queue changes
- Sync strategy: last-writer-wins; conflict markers

Acceptance
- Can create logs offline and see them in history; on reconnect, persist without duplication
