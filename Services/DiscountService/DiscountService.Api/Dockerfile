FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

ARG MOD
ENV MOD=${MOD}

# Copy csproj and restore as distinct layers
COPY DiscountService.sln .
COPY DiscountService.Abstractions/*.csproj ./DiscountService.Abstractions/
COPY DiscountService.Core/*.csproj ./DiscountService.Core/
COPY DiscountService.Api/*.csproj ./DiscountService.Api/

RUN dotnet restore DiscountService.Api/DiscountService.Api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish DiscountService.Api/DiscountService.Api.csproj -c $MOD -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
USER app
ENTRYPOINT ["dotnet", "DiscountService.Api.dll"]