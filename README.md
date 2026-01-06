# Todo App (.NET + Angular)

A simple Todo application demonstrating [k8s-ephemeral-environments](https://github.com/koder-cat/k8s-ephemeral-environments) with .NET 10 and Angular 21.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core 10 |
| Frontend | Angular 21 |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core |
| Styling | Tailwind CSS 4 |

## Project Structure

```
todo-app/
├── TodoApp.Api/          # ASP.NET Core Web API
│   ├── Controllers/      # API endpoints
│   ├── Models/           # Entity and DTOs
│   ├── Services/         # Business logic
│   ├── Data/             # DbContext
│   └── Migrations/       # EF Core migrations
├── todo-app-web/         # Angular frontend
│   └── src/app/          # Components and services
├── Dockerfile            # Multi-stage build
├── docker-compose.yml    # Local development
└── k8s-ee.yaml           # PR environment config
```

## Local Development

### Prerequisites

- .NET 10 SDK
- Node.js 22+
- Docker and Docker Compose

### Setup

```bash
cd todo-app

# Start PostgreSQL
docker compose up -d

# Run API (terminal 1)
cd TodoApp.Api
dotnet run

# Run Angular (terminal 2)
cd todo-app-web
npm install
npm start
```

Open http://localhost:4200

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/todos` | List all todos |
| POST | `/api/todos` | Create todo |
| PATCH | `/api/todos/{id}` | Update todo |
| DELETE | `/api/todos/{id}` | Delete todo |
| GET | `/api/health` | Health check |

## PR Environments

Each pull request automatically gets an isolated environment:

1. PR opened → Namespace created
2. App + PostgreSQL deployed
3. Preview URL: `todo-app-dotnet-pr-{number}.k8s-ee.genesluna.dev`
4. PR closed → Environment destroyed
