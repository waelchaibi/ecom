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
- Customer: register/login at `/account/register` and `/account/login`; checkout at `/order` (requires sign-in).
- Admin (JWT): full customer list/detail under `GET /api/admin/customers`, `GET /api/admin/customers/{id}`, `GET /api/admin/customers/{id}/orders`.

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
4. **Loyalty:** prior qualifying orders are **Confirmed** or **Shipped** only (not `Pending`/`Cancelled`). Place a first small order for customer `1` (no loyalty gift), then a second order → Loyalty Mug on the second order when rules and stock allow.
5. **Promotion:** enter promotion code `SPRINT2` on the order form (or send in JSON) → Promo Keychain when other rules also match, gifts are deduplicated by **gift** (same gift only once). Higher **Priority** on `GiftRules` is evaluated first.

## 11) Sprint 3 — admin space, analytics, JWT

### Admin login (development)

- Open `http://localhost:4200/login` for the single sign-in page, or `http://localhost:4200/admin` (redirects to `/login` if not signed in). The old path `/admin/login` redirects to `/login`.
- API credentials (see `EcommerceAPI/appsettings.Development.json` under `Admin`):
  - Username: `admin`
  - Password: `Admin123!`
- Token is stored in `localStorage` under `ecom_admin_jwt` and sent as `Authorization: Bearer` for `/api/admin/*` and `/api/analytics/*`.

### Backend highlights

- **Auth:** `POST /api/auth/login` — JWT with role `Admin` for protected routes.
- **Analytics:** `GET /api/analytics/dashboard?lowStockThreshold=10`, `GET /api/analytics/export/csv` (same query param). Revenue metrics exclude `Pending` and `Cancelled` orders.
- **Admin:** `POST /api/admin/products` (create); `PUT|DELETE|PATCH …/stock` on `api/admin/products/{id}`; `api/admin/gift-rules` (create, update, activate/deactivate, delete); `GET api/admin/orders` (paged), `GET api/admin/orders/{id}`; **`POST api/admin/orders/{id}/confirm-payment`** (Pending → Confirmed, applies gifts); **`POST api/admin/orders/{id}/cancel`** (restores product + gift stock); `GET api/admin/customers`, `GET api/admin/customers/{id}`, `GET api/admin/customers/{id}/orders`; `POST api/admin/gifts` (create catalog gifts).
- **Customer auth:** `POST /api/auth/customer/register`, `POST /api/auth/customer/login` (JWT role `Customer`); `GET /api/me`, `GET /api/me/orders`, `GET /api/me/orders/{id}`.
- **Emulated payment (no Stripe):** `POST /api/me/orders/{id}/pay` — customer simulates payment → order **Confirmed**, gifts applied. `POST /api/me/orders/{id}/cancel` for **Pending** orders only.
- **Storefront:** public `GET /api/products`; **`POST /api/orders`** (customer JWT) creates **Pending** orders. Admin **confirm-payment** still works as a back-office alternative.

### Angular admin UI

After `npm start`, use the **Admin** link in the header (you are sent to `/login` when not signed in) or browse directly to `/login`. Sections: dashboard (KPIs + tables + CSV export), products (category + image URL), categories, gift rules, orders (filters in URL), audit log, customers (with per-customer order history).

### Demo checklist

1. `dotnet run` in `EcommerceAPI`, `npm start` in `ecommerce-web`.
2. Sign in at `/login` with dev credentials above.
3. Open **Dashboard** — confirm metrics load; try **Export CSV**.
4. **Products** — use **Add product** (admin JWT), edit, **Set stock**, delete only when no order lines (409 otherwise). Concurrent checkouts that lose the stock race return **409** with a retry message.
5. **Gift rules** — toggle active/inactive, create a test rule, optional **Add gift**.
6. **Orders / emulated payment** — place an order (**Pending**), then click **Pay now (simulate)** on checkout, **My orders**, or order detail — or use **Admin → Confirm payment** as an alternative. Gifts and revenue update after payment. Cancel a **Pending** order from the customer UI to release stock.
7. **Customers** — open **Order history** for a customer.

### Security note

Change `Jwt:Secret` and `Admin:Password` for any non-local deployment; empty admin password returns `503` from the login endpoint.

## 12) Expansion roadmap (post–Sprint 3)

### Done

| Area | Highlights |
|------|------------|
| **Security & orders** | Admin-only sensitive APIs; pending orders; atomic stock; admin confirm/cancel; analytics excludes pending/cancelled |
| **Customer accounts** | Register/login JWT; `GET /api/me`, my orders; checkout requires customer auth |
| **Emulated payment** | `POST /api/me/orders/{id}/pay` (no Stripe); gifts on confirm |
| **Catalog & cart** | Categories, product `imageUrl`, server cart (`/api/me/cart`), tax/shipping on orders, storefront category filter |
| **Admin** | Order list filters (status, customer search/ID, date range); categories CRUD; products with category + image URL; **audit log** API + UI |
| **Hardening & ops** | `AuditLogs` table; rate limits on auth/orders/pay; optional `AdminSecurity:AllowedIps`; `GET /health`; `X-Correlation-ID`; JSON logging (Development) |

### API additions (recent)

- `GET /api/admin/audit-logs` — paged audit entries (`action`, `entityType`, `fromUtc`, `toUtc`)
- `GET /api/admin/orders` — filters: `status`, `customerSearch`, `customerId`, `fromUtc`, `toUtc`
- Storefront: `GET /api/categories`, `GET /api/products?categoryId=`, cart under `/api/me/cart`

### Still open (suggested next)

1. **Payments** — real PSP (Stripe/webhooks) if needed beyond simulate.
2. **Catalog** — image upload (not just URL), product search/pagination on storefront.
3. **Cart** — guest cart / merge on login.
4. **Customer** — password reset, email notifications.
5. **Production** — rotate secrets, CORS, disable Swagger, automated tests.
6. **Analytics** — date-range filters on admin dashboard (orders already support ranges).
