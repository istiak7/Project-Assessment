##  Concurrent Request handle

  In-memory synchronization mechanisms in C# lock, SemaphoreSlim etc only work within a single process.
  so, if we consider multi-process we have to look Database-level Row-Level row Locking with transaction scope. we also can consider redis lock.
  PostgreSQL handles request automatically FIFO.
  Database row-level locks ensure that concurrent requests
  Thousands of applicants  can simultaneously, try to access available class seats.

## Atomic Transactions
  - In a same lock, apply db row lock, check seat availiblity, then held.consider in a transaction.

## Hold Expiry & Automatic Seat Release

To ensure expired seats are immediately available without requiring any API hit, two layer strategy is implement

 Background Service 
   - BackgroundService runs every 10 seconds to scan for active HELD  whose expiresAt timestamp is less than DateTime.UtcNow
   -Identified records are updated to EXPIRED, releasing seats back.

Query-Level check
   - To prevent wrong seat counts during the 10-second background interval, all GET /classes and GET /classes/{classId} requests dynamically ignore or exclude expired held seats.
   - Any attempt to confirm an expired hold before the background worker cleans it up will be rejected at the API query level.


## student id is stored in local storage, that's why when page refresh or shut down browser, no affect to see apply details.

## Added polling using javascript timer function, that's why every 5s page automatically refresh,and user will see updated data like available seat, held seat.

## Check Uniqueness by studentId,classId and held status, when confirm request multiple times from user.

* Only admin can create class,now it just hardcoded a password, and handle with frontend.no jwt or rolebased auth use.