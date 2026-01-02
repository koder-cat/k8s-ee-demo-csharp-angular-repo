# Stack-Specific Challenges: .NET + Angular

When adapting this stack for k8s-ephemeral-environments, we encountered several issues that differ from the Node.js/React reference implementation.

## 1. Npgsql Connection String Format

**Problem:** The platform injects `DATABASE_URL` in PostgreSQL URI format (`postgresql://user:pass@host:port/db`), but Npgsql (the .NET PostgreSQL driver) expects ADO.NET connection string format (`Host=...;Database=...;Username=...;Password=...`).

**Symptom:**
```
System.ArgumentException: Format of the initialization string does not conform to specification starting at index 0.
```

**Solution:** Use the individual `PG*` environment variables instead of `DATABASE_URL`:

```csharp
var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var connectionString = pgHost != null
    ? $"Host={pgHost};Port={Environment.GetEnvironmentVariable("PGPORT") ?? "5432"};Database={Environment.GetEnvironmentVariable("PGDATABASE") ?? "app"};Username={Environment.GetEnvironmentVariable("PGUSER") ?? "app"};Password={Environment.GetEnvironmentVariable("PGPASSWORD") ?? "app"}"
    : builder.Configuration.GetConnectionString("DefaultConnection");
```

## 2. Environment Variable Priority

**Problem:** ASP.NET Core's `GetConnectionString()` reads from `appsettings.json` before environment variables, causing the app to use `localhost` instead of the injected database host.

**Symptom:**
```
Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5432
Connection refused
```

**Solution:** Check environment variables first, then fall back to configuration:

```csharp
// Wrong - config takes precedence
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// Correct - env vars take precedence for k8s
var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var conn = pgHost != null ? BuildFromEnvVars() : GetFromConfig();
```

## 3. Multi-Architecture Docker Builds

**Problem:** The target VPS runs on ARM64 (Oracle Cloud), but .NET builds default to the host architecture.

**Symptom:** Container crashes with exec format error or architecture mismatch.

**Solution:** Specify the runtime identifier explicitly in the Dockerfile:

```dockerfile
# Restore with target RID
RUN dotnet restore -r linux-musl-arm64

# Publish with target RID
RUN dotnet publish -c Release -r linux-musl-arm64 --no-restore -o /app/publish
```

## 4. Angular Build Output Path

**Problem:** Angular's `application` builder (default in v17+) outputs to `dist/{project}/browser/` instead of `dist/{project}/`.

**Symptom:** 404 errors for static files, `index.html` not found.

**Solution:** Copy from the correct path in Dockerfile:

```dockerfile
# Old path (v16 and earlier)
COPY --from=frontend-builder /app/dist/todo-app-web ./wwwroot/

# New path (v17+ with application builder)
COPY --from=frontend-builder /app/dist/todo-app-web/browser ./wwwroot/
```

## 5. Tailwind CSS 4 Configuration

**Problem:** Tailwind CSS 4 uses CSS-first configuration instead of `tailwind.config.js`. Old tutorials and examples don't work.

**Solution:** Configure directly in `styles.css`:

```css
@import "tailwindcss";

@theme {
  /* Custom theme overrides */
}
```

No `tailwind.config.js` or `postcss.config.js` needed.

## Key Differences from Node.js Stack

| Aspect | Node.js | .NET |
|--------|---------|------|
| DB Connection | URI format works | Requires ADO.NET format |
| Config Priority | Env vars override config | Config file takes precedence |
| Build Target | Auto-detects arch | Needs explicit RID |
| Static Files | Express `static()` | `UseStaticFiles()` + SPA fallback |
| Hot Reload | Built-in | `dotnet watch` required |

## Environment Variables Consumed

| Variable | Used For |
|----------|----------|
| `PGHOST` | PostgreSQL host (service name in k8s) |
| `PGPORT` | PostgreSQL port (default: 5432) |
| `PGDATABASE` | Database name |
| `PGUSER` | Database username |
| `PGPASSWORD` | Database password |
| `CORS_ORIGIN` | Allowed CORS origins (comma-separated) |

**Note:** `DATABASE_URL` is NOT used because Npgsql doesn't support URI format.
