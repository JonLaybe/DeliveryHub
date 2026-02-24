# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- Копируем только файлы проектов для кэширования NuGet ---

COPY Services/CatalogService/Catalog.API/Catalog.API.csproj Services/CatalogService/Catalog.API/
COPY Services/CatalogService/Catalog.Application/Catalog.Application.csproj Services/CatalogService/Catalog.Application/
COPY Services/CatalogService/Catalog.Domain/Catalog.Domain.csproj Services/CatalogService/Catalog.Domain/
COPY Shared/Shared.Web/Shared.Web.csproj Shared/Shared.Web/

# Restore только API (подтянет зависимости остальных проектов)
RUN dotnet restore Services/CatalogService/Catalog.API/Catalog.API.csproj

# --- Копируем весь исходный код ---
COPY . .

# Publish
RUN dotnet publish Services/CatalogService/Catalog.API/Catalog.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Порт ASP.NET Core (лучше стандартный)
EXPOSE 8080

ENTRYPOINT ["dotnet", "Catalog.API.dll"]