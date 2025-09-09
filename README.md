### LinkDev Talabat — Full‑Stack E‑Commerce (ASP.NET Core 8 + Angular 11)

A modular, production‑ready clone of Talabat style ordering with a clean architecture backend and an Angular SPA frontend. Includes authentication/authorization (JWT), product catalog with filtering/sorting/pagination, basket, orders, payment (Stripe), caching (Redis), and Swagger API docs.

---

## Overview
- **Backend**: ASP.NET Core 8 Web API, EF Core, Identity, Redis, Stripe
- **Frontend**: Angular 11, Bootstrap 4, Toastr, Breadcrumbs
- **Architecture**: Clean architecture with separate projects for Domain, Application, Infrastructure, API, and MVC Dashboard

---

## Solution Structure
```text
LinkDev.Talabat-backup/
├─ LinkDev.Talabat.sln
├─ LinkDev.Talabat.Core.Domain/                # Entities, Specifications, Contracts
├─ LinkDev.Talabat.Core.Application/           # Use cases, services, mapping
├─ LinkDev.Talabat.Core.Application.Abstraction/ # Interfaces, DTOs, models
├─ LinkDev.Talabat.Infrastructure/             # Infrastructure services (payments, cache)
├─ LinkDev.Talabat.Infrastructure.Persistence/ # EF Core, DbContexts, repos, seeding
├─ LinkDev.Talabat.APIs/                       # ASP.NET Core Web API host
├─ LinkDev.Talabat.APIs.Controllers/           # API controllers library
├─ LinkDev.Talabat.Dashboard/                  # MVC dashboard (ancillary)
└─ Client/                                     # Angular 11 SPA
```

---

## Tech Stack
- **API**: .NET 8, ASP.NET Core, EF Core, Identity, AutoMapper, Swashbuckle (Swagger)
- **Data**: SQL Server, Redis
- **Payments**: Stripe
- **Frontend**: Angular 11, RxJS 6, Bootstrap 4, ngx‑toastr, ngx‑spinner, xng‑breadcrumb

---

## Prerequisites
- **.NET SDK 8.0**
- **SQL Server** (LocalDB or full) and a user with DB create permissions
- **Redis** running locally on `localhost:6379`
- **Node.js 14.x** and **Angular CLI ~11** (installed locally via `npm i`)

---

## Configuration
Backend configuration lives in `LinkDev.Talabat.APIs/appsettings.json` (override with `appsettings.Development.json` or env vars):

```json
{
  "ConnectionStrings": {
    "StoreContext": "Server=.;Database=Talabat;Trusted_Connection=True;TrustServerCertificate=True;",
    "IdentityContext": "Server=.;Database=Talabat.Identity;Trusted_Connection=True;TrustServerCertificate=True;",
    "Redis": "localhost"
  },
  "Urls": {
    "ApiBaseUrl": "https://localhost:7162",
    "FrontendUrl": "https://localhost:4200"
  },
  "jwtSettings": {
    "Key": "<strong-secret>",
    "Audience": "TalabatUsers",
    "Issuer": "TalabatIdentity",
    "DurationInMinutes": 10
  },
  "StripeSettings": {
    "PublishableKey": "<stripe-publishable-key>"
  }
}
```

Notes:
- The API enables CORS for `http://localhost:4200` and `https://localhost:4200`. Adjust `Urls:FrontendUrl` or CORS policy if needed.
- JWT `Key` should be set from a secure secret store in production.
- Port numbers may differ on your machine based on launch profiles.

---

## Getting Started

### 1) Clone and restore
```bash
# Windows PowerShell
cd C:\Users\<you>\source\repos
# clone repo first if needed, then:
cd LinkDev.Talabat-backup
```

Restore .NET projects:
```bash
# from repository root
dotnet restore LinkDev.Talabat.sln
```

Install frontend deps:
```bash
cd Client
npm install
cd ..
```

### 2) Configure local settings
- Ensure SQL Server is running and accessible via the connection strings in `LinkDev.Talabat.APIs/appsettings.json`.
- Ensure Redis is running: default `localhost:6379`.
- Optionally update `Urls:ApiBaseUrl` and `Urls:FrontendUrl` to match your ports.
- Set `StripeSettings:PublishableKey` (and secret key in your Stripe dashboard as needed) for payment flows.

### 3) Run the API
```bash
# from repository root
cd LinkDev.Talabat.APIs
dotnet run
```
Default endpoints:
- Swagger UI: `https://localhost:7162/swagger`
- API base: `https://localhost:7162`

The database is initialized and seeded automatically on startup.

### 4) Run the Angular SPA
```bash
# from repository root
cd Client
npm start
# opens http://localhost:4200
```

> Ensure the API is running before the SPA to avoid CORS/network errors.

---

## Useful Scripts
- API: `dotnet build`, `dotnet run` inside `LinkDev.Talabat.APIs`
- Client: `npm start`, `npm run build`, `npm test`

---

## Project Highlights
- Clean separation of concerns: Domain, Application, Infrastructure, API
- Global exception handling and consistent API error responses
- Caching with Redis and attribute‑based response caching
- JWT‑based authentication and authorization
- Configurable CORS policy for local and production
- Swagger/OpenAPI for interactive API documentation

---

## Troubleshooting
- **SQL Server connection**: Verify the instance name in `ConnectionStrings`. For LocalDB try `Server=(localdb)\\MSSQLLocalDB;`.
- **Redis not reachable**: Ensure a local Redis instance is running (`localhost:6379`).
- **CORS errors**: API must be running; confirm `Program.cs` CORS policy and `Urls:FrontendUrl`.
- **Port mismatch**: Update `Urls:ApiBaseUrl` and Angular environment files if ports differ.
- **SSL/cert warnings**: Dev certificates may need trust: `dotnet dev-certs https --trust`.

---

## License
This repository is provided for educational and demonstration purposes. Add your preferred license here.
