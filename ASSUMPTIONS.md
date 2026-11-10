##  Concurrent Request
Requirement : Thousands of applicants  simultaneously, try to access available class seats.
My Thinking : In-Memory vs Database Locking 
  - In-memory synchronization mechanisms C# lock, SemaphoreSlim only work within a single process.
  - To support multi-process we have to look Database-level Row-Level Locking.
  - PostgreSQL handles request automatically FIFO.
  - Database row-level locks ensure that concurrent requests

## Atomic Transactions
  - In a same lock, apply db row lock, check seat availiblity, then held.consider in a transaction.

## Expiry & Automatic Seat Release

To ensure expired seats are immediately available without requiring any API triggers, two layer strategy is implement
  Background Service 
   - BackgroundService runs every 10 seconds to scan for active HELD  whose expiresAt timestamp is less than DateTime.UtcNow
   -Identified records are updated to EXPIRED, releasing seats back.
  Query-Level
   - To prevent wrong seat counts during the 10-second background interval, all `GET /classes` and `GET /classes/{classId}` requests dynamically ignore or exclude expired `HELD` seats.
   - Any attempt to confirm an expired hold before the background worker cleans it up will be rejected at the API query level.

## At a Time a student no confirm multiple time in a class application
- Check stuentId,classId and held status uniqueness.

