# Stage 1: Build Angular
FROM node:22-alpine AS frontend-builder
WORKDIR /app
COPY todo-app-web/package*.json ./
RUN npm ci
COPY todo-app-web/ ./
RUN npm run build
# Output: dist/todo-app-web/browser/

# Stage 2: Build .NET
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS backend-builder
WORKDIR /app
COPY TodoApp.Api/*.csproj ./
RUN dotnet restore -r linux-musl-arm64
COPY TodoApp.Api/ ./
COPY --from=frontend-builder /app/dist/todo-app-web/browser ./wwwroot/
RUN dotnet publish -c Release -r linux-musl-arm64 --no-restore -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 appuser
COPY --from=backend-builder --chown=appuser:dotnet /app/publish ./
USER appuser
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["./TodoApp.Api"]
