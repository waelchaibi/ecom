# Ecom Project - Local Setup (Windows)

This repository contains:

- `EcommerceAPI` (ASP.NET Core backend)
- `ecommerce-web` (Angular frontend)
- PostgreSQL database

This guide documents the exact prerequisite setup and boot flow we used.

## 1) Prerequisites

Install these tools on Windows:

- Node.js + npm
- .NET SDK 6
- PostgreSQL 16
- Chocolatey (package manager)

## 2) Install Chocolatey (if missing)

Run **PowerShell as Administrator**:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; `
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; `
iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
```

## 3) Install prerequisites with Chocolatey

```powershell
choco install -y nodejs-lts
choco install -y dotnet-6.0-sdk
choco install -y postgresql16 --params "'/Password:postgres /Port:5432'"
refreshenv
```

## 4) Verify installs

```powershell
node -v
npm -v
dotnet --version
& "C:\Program Files\PostgreSQL\16\bin\psql.exe" --version
```

## 5) PostgreSQL first-time setup

Use the default credentials configured above:

- User: `postgres`
- Password: `postgres`
- Port: `5432`

Create the database:

```powershell
& "C:\Program Files\PostgreSQL\16\bin\psql.exe" -h 127.0.0.1 -U postgres -d postgres -c "CREATE DATABASE ecommerce;"
```

## 6) Run the project

Open two terminals from repo root.

### Backend

```powershell
cd EcommerceAPI
dotnet restore
dotnet run
```

API endpoints:

- `http://localhost:5069`
- `https://localhost:7018`
- Swagger: `https://localhost:7018/swagger`
- Customers (read-only for storefront): `GET /api/customers`, `GET /api/customers/{id}`

### Frontend

```powershell
cd ecommerce-web
npm install
npm start
```

Frontend URL:

- `http://localhost:4200`

## 7) HTTPS certificate fix (if browser shows cert errors)

If you see `ERR_CERT_AUTHORITY_INVALID`:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Then restart backend and browser.

## 8) Common troubleshooting

### A) `npm` or `dotnet` not recognized

- Close and reopen terminal after install.
- Run `refreshenv`.
- Recheck versions.

### B) PostgreSQL password authentication failed

This usually means old data directory credentials were reused.

Quick reset flow:

1. Stop service:

```powershell
net stop postgresql-x64-16
```

2. Temporarily set localhost auth to `trust` in:

- `C:\Program Files\PostgreSQL\16\data\pg_hba.conf`

Change these lines:

```text
host    all    all    127.0.0.1/32    trust
host    all    all    ::1/128         trust
```

3. Start service and reset password:

```powershell
net start postgresql-x64-16
& "C:\Program Files\PostgreSQL\16\bin\psql.exe" -h 127.0.0.1 -U postgres -d postgres -c "ALTER USER postgres WITH PASSWORD 'postgres';"
```

4. Revert `pg_hba.conf` auth back to `scram-sha-256`, then restart service.

### C) Angular error `Http failure response ... 0 undefined`

This can happen when API HTTP is redirected to HTTPS and cert trust fails.

Use HTTPS API base URL in frontend environment:

- `https://localhost:7018/api`

And trust the .NET dev certificate (step 7).

### D) Port already in use (`4200`, `5069`, `7018`)

- Another server instance is already running.
- Stop old process, or reuse the running instance.

## 9) Current expected dev config

- PostgreSQL connection string (backend dev):
  - `Host=localhost;Database=ecommerce;Username=postgres;Password=postgres`
- Frontend API base URL:
  - `https://localhost:7018/api`

## 10) Sprint 2 — promotional gifts (testing)

After `dotnet run`, migrations add `Gifts`, `GiftRules`, and `OrderGifts`. Seeded rules (see migration `AddSprint2PromotionalGifts`):

| Rule | Type | Condition | Gift |
|------|------|-----------|------|
| 1 | Amount | order total strictly greater than `100` (decimal in `ConditionValue`) | Premium Gift Pack |
| 2 | Loyalty | prior completed orders for customer strictly greater than `0` (from the 2nd order onward) | Loyalty Mug |
| 3 | Promotion | promo code `SPRINT2` (case-insensitive) | Promo Keychain |

Optional API: `POST /api/orders` body may include `"promotionCode": "SPRINT2"`.

Quick checks:

1. Run API so migrations apply: `cd EcommerceAPI` then `dotnet run`.
2. `GET https://localhost:7018/api/gifts` and `GET https://localhost:7018/api/giftrules` — confirm seeded data.
3. **Amount only:** customer `1`, one line e.g. Laptop ×1 (total above 100) → response `assignedGifts` includes Premium Gift Pack; gift stock decreases.
4. **Loyalty:** place a first small order for customer `1` (no loyalty gift), then a second order (any total) → Loyalty Mug on the second order if stock available.
5. **Promotion:** enter promotion code `SPRINT2` on the order form (or send in JSON) → Promo Keychain when other rules also match, gifts are deduplicated by **gift** (same gift only once). Higher **Priority** on `GiftRules` is evaluated first.

Manage data (no admin UI in Sprint 2): `POST /api/gifts`, `POST /api/giftrules`, `PUT /api/giftrules/{id}`.
