# Freight Shipment Tracker

A small freight management app. The backend is an ASP.NET Core REST API; the frontend is an
Angular single-page app that consumes it. You can create shipments, track them through their
lifecycle, update status (with valid-transition rules enforced server-side), delete, and
filter/search.

## Tech stack

- **Backend:** ASP.NET Core 8 Web API (C#), Entity Framework Core with SQLite
- **Frontend:** Angular 21 (standalone components), reactive forms, RxJS
- **Tests:** xUnit (status-transition rules)

## Project structure

```
backend/
  FreightTracker.API/         REST API (Controllers -> Services -> Repositories)
  FreightTracker.API.Tests/   Unit tests for the transition logic
frontend/                     Angular app
```

The backend is organised in layers:

- **Controller** (`ShipmentsController`) — HTTP routing and status codes only.
- **Service** (`ShipmentService`, `ShipmentStatusService`) — business rules: tracking-number
  generation, DTO mapping, and the status-transition rule.
- **Repository** (`ShipmentRepository`) — all EF Core data access.

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm
- Angular CLI (`npm install -g @angular/cli`) — optional; `npm start` works without a global install

## Running the backend

```bash
cd backend/FreightTracker.API
dotnet run
```

The API listens on `http://localhost:5243`. On first run it creates a SQLite database
(`freight.db`) and seeds a handful of sample shipments. CORS is configured to allow the Angular
dev server at `http://localhost:4200`.

## Running the frontend

In a second terminal:

```bash
cd frontend
npm install   # first time only
npm start
```

The app runs at `http://localhost:4200`. It expects the API at `http://localhost:5243`
(set in `frontend/src/app/services/shipment.service.ts`).

## API endpoints

| Method | Route                          | Purpose                                              |
| ------ | ------------------------------ | ---------------------------------------------------- |
| GET    | `/api/shipments`               | List shipments (`?status=`, `?carrier=`, `?search=`) |
| GET    | `/api/shipments/{id}`          | Get a single shipment                                |
| POST   | `/api/shipments`               | Create (server generates the tracking number)        |
| PUT    | `/api/shipments/{id}`          | Update shipment details                              |
| PATCH  | `/api/shipments/{id}/status`   | Update status (valid-transition rule enforced)       |
| DELETE | `/api/shipments/{id}`          | Delete a shipment                                    |
| GET    | `/api/shipments/{id}/history`  | Status-change timeline                               |

## Status transition rule

Status must follow this lifecycle; invalid jumps are rejected with a `400` and a clear message:

```
Pending    -> InTransit | Cancelled
InTransit  -> Delivered | Cancelled
Delivered  -> (terminal)
Cancelled  -> (terminal)
```

The rule lives in `ShipmentStatusService` and is covered by unit tests. The Angular UI only offers
valid next statuses, but the server is the source of truth.

## Running the tests

```bash
cd backend/FreightTracker.API.Tests
dotnet test
```

## Assumptions & tradeoffs

- **SQLite via `EnsureCreated()`**, not migrations. This keeps setup to zero, but it means the
  schema is only built once. **After a schema change, delete `freight.db` so the tables are
  recreated.** A real app would use EF Core migrations.
- **Decimal stored as TEXT** in SQLite (SQLite has no native decimal type), to preserve precision.
- **API URL is hardcoded** in the Angular service. For a real deployment this belongs in an
  Angular `environment.ts` file.
- **Tracking numbers** are `FRT-{year}-{sequence}`, generated from the row count with a
  uniqueness check. Fine at this scale; a high-throughput system would use a database sequence.
- **Carrier filter is an exact match** (the dropdown is populated from existing data), while the
  free-text search on origin/destination is a case-insensitive partial match.
- **The transition map is duplicated on the client** for UX (to only show valid next statuses).
  It's purely a hint — the server still enforces the rule.
- **Delete and status-change confirmations** use the browser's native `confirm()` dialog.

## What I'd improve with more time

- EF Core migrations instead of `EnsureCreated()`.
- Move the API URL into Angular environment files.
- Pagination and sorting on the shipment list.
- Persist the active filters in the URL query params.
- A summary widget (counts per status) on the dashboard.
- Replace native `confirm()` dialogs with a styled modal.
- More tests: controller/service integration tests alongside the transition unit tests.
