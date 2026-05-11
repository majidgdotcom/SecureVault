# SecureVault

A full-stack encrypted note-taking application built with Clean Architecture and AES-256 encryption.

## Architecture

```
SecureVault/
├── SecureVault.Domain/          # Entities, interfaces, domain logic
├── SecureVault.Application/     # Use cases, services, DTOs
├── SecureVault.Infrastructure/  # EF Core, encryption, repositories
├── SecureVault.API/             # ASP.NET Core Web API
└── SecureVault.Web/             # React + TypeScript frontend
```

Dependencies flow inward — Domain has no external dependencies; outer layers depend on inner ones.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8 Web API |
| ORM | Entity Framework Core + SQL Server |
| Encryption | AES-256-CBC with PBKDF2 key derivation |
| Frontend | React 19 + TypeScript |
| Frontend pattern | MVVM (custom hooks as ViewModels) |
| CI/CD | GitHub Actions (build + security scan) |

## Features

- AES-256-CBC encryption with a unique IV per note
- PBKDF2 key derivation (100,000 iterations, SHA-256)
- Full CRUD for notes via REST API
- Result pattern for structured error handling (no exception leakage)
- Unit of Work + Repository pattern
- CancellationToken support throughout
- CI pipeline with `dotnet` vulnerability scanning and `npm audit`

## Known Limitations

- **No authentication layer** — endpoints accept a plain `userId` string. A production version would add JWT-based auth (e.g. ASP.NET Core Identity or Auth0) so users can only access their own notes.
- **No backend tests** — unit tests for `NotesService` and `EncryptionService` are the natural next step.
- **Single-user assumption** — no multi-tenancy or role-based access control.

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (local or Docker)
- Node.js 20+

### Backend

```bash
# 1. Set your encryption key — do NOT use the placeholder in production
#    Option A: dotnet user-secrets (recommended for local dev)
cd SecureVault.API
dotnet user-secrets set "Encryption:MasterKey" "your-random-32+-char-secret-here"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"

#    Option B: environment variables
export Encryption__MasterKey="your-random-32+-char-secret-here"

# 2. Apply migrations and run
dotnet ef database update --project SecureVault.Infrastructure --startup-project SecureVault.API
dotnet run --project SecureVault.API
```

API runs at `http://localhost:5043` by default.

### Frontend

```bash
cd SecureVault.Web
cp .env.example .env          # fill in REACT_APP_API_URL
npm install
npm start
```

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/notes` | Create a note |
| `GET` | `/api/notes/{id}` | Get a note by ID |
| `GET` | `/api/notes/user/{userId}` | Get all notes for a user |
| `PUT` | `/api/notes/{id}` | Update a note |
| `DELETE` | `/api/notes/{id}` | Delete a note |

## Security Notes

- `appsettings.json` contains a **placeholder key** — never use it as-is
- Use `dotnet user-secrets` for local development or environment variables in production
- `.env` is gitignored — use `.env.example` as the template
