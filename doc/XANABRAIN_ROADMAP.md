# XanaBrain Implementation Roadmap

**Objective:** migrate logic from Mobile to Cloud, creating a robust "Brain" for the ecosystem.

---

## Phase 1: The Foundation (Identity)
**Goal:** Establish trust between Flutter and .NET.

1.  **Database Config:**
    *   Create `User` entity in `Domain`.
    *   Setup EF Core Migration to create `Users` table in PostgreSQL.
2.  **Auth Pipeline:**
    *   Implement `AuthMiddleware` in Infrastructure.
    *   **Logic:** Decrypt Firebase JWT Header -> Extract UID -> Check if exists in DB ?? Create it.
3.  **Endpoint:** `GET /api/me`
    *   Returns current user profile from PostgreSQL.
    *   *Success Indicator:* Flutter calls this endpoint with a Firebase Token and gets a 200 OK json.

---

## Phase 2: Inventory Migration (The "Search" Problem)
**Goal:** Remove local filtering from the mobile app.

1.  **Domain:**
    *   Create `Instrument` entity (OwnerId, Name, Type, CreatedAt).
2.  **Application:**
    *   Create `CreateInstrumentCommand` (Validator: Name required).
    *   Create `GetInstrumentsQuery` (Accepts `SearchTerm`, `Page`, `PageSize`).
3.  **Infrastructure:**
    *   Implement Repository using EF Core `.Where(x => x.Name.Contains(term))`.
4.  **API:**
    *   `POST /api/instruments` (Sync data from mobile initially).
    *   `GET /api/instruments?search=fender`.

---

## Phase 3: The "Brain" (Maintenance Logic)
**Goal:** Centralize business rules invisible to the user.

1.  **Domain:**
    *   Create `MaintenanceLog` entity.
    *   Add Method: `Instrument.AddUsageHours(hours)`.
    *   Add Logic: `bool NeedsMaintenance => usagesHours > 100`.
2.  **Application:**
    *   Create `RegisterSessionCommand` (Input: Duration).
    *   **Logic:**
        *   Update Instrument usage.
        *   Check `NeedsMaintenance`.
        *   If true -> Dispatch `SendPushNotificationCommand`.
3.  **API:**
    *   `POST /api/sessions`.

---

## Phase 4: Clean Up (The "Switch")
**Goal:** Turn off the old logical paths in Flutter.

1.  **Mobile:** Replace `InventoryRemoteDataSource` (Direct Firestore) with `InventoryApiService` (Http .NET).
2.  **Mobile:** Remove `where()` logic from UI. Trust the API response.
3.  **Mobile:** Remove local maintenance calculation. Show alert only if API says `maintenance_required: true`.
