FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AuthService.sln ./
COPY src/Auth.Api/Auth.Api.csproj src/Auth.Api/
COPY src/Auth.Application/Auth.Application.csproj src/Auth.Application/
COPY src/Auth.Domain/Auth.Domain.csproj src/Auth.Domain/
COPY src/Auth.Infrastructure/Auth.Infrastructure.csproj src/Auth.Infrastructure/

COPY tests/Auth.UnitTests/Auth.UnitTests.csproj tests/Auth.UnitTests/
COPY tests/Auth.IntegrationTests/Auth.IntegrationTests.csproj tests/Auth.IntegrationTests/

RUN dotnet restore

COPY . .
RUN dotnet publish src/Auth.Api/Auth.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
 && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
 && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Auth.Api.dll"]