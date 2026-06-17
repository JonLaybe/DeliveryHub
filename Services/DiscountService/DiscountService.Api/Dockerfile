FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

ARG MOD
ENV MOD=${MOD}

# Copy csproj and restore as distinct layers
COPY Services/DiscountService/DiscountService.sln Services/DiscountService/
COPY Services/DiscountService/DiscountService.Abstractions/DiscountService.Abstractions.csproj Services/DiscountService/DiscountService.Abstractions/
COPY Services/DiscountService/DiscountService.Core/DiscountService.Core.csproj Services/DiscountService/DiscountService.Core/
COPY Services/DiscountService/DiscountService.Api/DiscountService.Api.csproj Services/DiscountService/DiscountService.Api/
COPY Shared/Shared.Domain/Shared.Domain.csproj Shared/Shared.Domain/

# Копируем остальной код
COPY . ./


RUN dotnet restore Services/DiscountService/DiscountService.Api/DiscountService.Api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish Services/DiscountService/DiscountService.Api/DiscountService.Api.csproj -c $MOD -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
USER app
ENTRYPOINT ["dotnet", "DiscountService.Api.dll"]