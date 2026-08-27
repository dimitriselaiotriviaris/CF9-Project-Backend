# CF9 Project Backend

ASP.NET Core backend for the CF9 game platform.

The backend provides cookie-based authentication, role-based authorization, company game management, gamer libraries, Entity Framework Core persistence, and SQL Server database migrations.

## Tech Stack

- .NET 10
- ASP.NET Core
- Entity Framework Core 10
- Microsoft SQL Server / SQL Server Express
- Cookie Authentication
- AutoMapper
- BCrypt.Net
- Serilog

## Roles

| Role ID | Role |
| ---: | --- |
| 1 | `ADMIN` |
| 2 | `COMPANY` |
| 3 | `GAMER` |

## Main Data Model

- `Users`
- `Roles`
- `Companies`
- `Gamers`
- `Games`
- `GamersGames`
- `Capabilities`
- `RolesCapabilities`

A `User` belongs to one `Role`. Company accounts link to `Companies`, gamer accounts link to `Gamers`, games belong to companies, and `GamersGames` is the many-to-many table used for gamer libraries.

## API Overview

### Authentication

```text
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
POST /api/auth/logout
```

### Company

Requires the `COMPANY` role.

```text
GET  /api/company/games
POST /api/company/games
PUT  /api/company/games/{id}
```

### Gamer

Requires the `GAMER` role.

```text
GET    /api/gamer/games
GET    /api/gamer/library
POST   /api/gamer/library/{gameId}
DELETE /api/gamer/library/{gameId}
```

## Installation

### Requirements

Install:

1. [.NET 10 SDK](https://dotnet.microsoft.com/)
2. Microsoft SQL Server or SQL Server Express
3. Git

Optional EF CLI:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Clone

```bash
git clone https://github.com/dimitriselaiotriviaris/CF9-Project-Backend.git
cd CF9-Project-Backend
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Configure SQL Server

The application currently expects a connection string named `DevConnection`.

Example `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DevConnection": "Server=localhost\\SQLEXPRESS;Database=CF9ProjectContext;User Id=YOUR_SQL_USER;Password=YOUR_SQL_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

Do not commit real deployment credentials.

You can instead use an environment variable.

PowerShell:

```powershell
$env:ConnectionStrings__DevConnection="Server=localhost\SQLEXPRESS;Database=CF9ProjectContext;User Id=YOUR_SQL_USER;Password=YOUR_SQL_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True"
```

Linux/macOS:

```bash
export ConnectionStrings__DevConnection='Server=localhost;Database=CF9ProjectContext;User Id=YOUR_SQL_USER;Password=YOUR_SQL_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True'
```

### 4. Apply database migrations

```bash
dotnet ef database update
```

The migrations create the schema and seed these roles:

```text
1  ADMIN
2  COMPANY
3  GAMER
```

Verify with:

```sql
SELECT Id, Name
FROM Roles
ORDER BY Id;
```

### 5. Trust the development HTTPS certificate

```bash
dotnet dev-certs https --trust
```

### 6. Run

```bash
dotnet run
```

The current Angular development frontend expects the backend at approximately:

```text
https://localhost:7259
```

Check `Properties/launchSettings.json` or console output if your development port differs.

## CORS and Cookies

The development backend allows credentialed requests from:

```text
http://localhost:4200
https://localhost:4200
```

The Angular frontend sends `withCredentials: true`, allowing the ASP.NET authentication cookie to be included.

If the frontend is deployed on another origin, update the CORS policy in `Program.cs`.

## Database Changes

When models change:

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

The project uses migrations; do not mix this with `EnsureCreated()`.

## HTTP Authentication Responses

API requests return:

- `401 Unauthorized` when no valid authenticated session exists.
- `403 Forbidden` when the user is authenticated but lacks the required role.

## Related Frontend
https://github.com/dimitriselaiotriviaris/project-frontend

https://github.com/dimitriselaiotriviaris/project-frontend
