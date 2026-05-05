FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
    
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Services/OrderService/Order.WebAPI/OrderService.WebAPI.csproj Services/OrderService/Order.WebAPI/
COPY Services/OrderService/Order.Core/OrderService.Core.csproj Services/OrderService/Order.Core/
COPY Services/OrderService/Order.Domain/OrderService.Domain.csproj Services/OrderService/Order.Domain/
COPY Services/OrderService/Order.Infrastructure/OrderService.Infrastructure.csproj Services/OrderService/Order.Infrastructure/
COPY Shared/Shared.RabbitMq/Shared.RabbitMq.csproj Shared/Shared.RabbitMq/

RUN dotnet restore "Services/OrderService/Order.WebAPI/OrderService.WebAPI.csproj"

COPY . .

WORKDIR "/src/Services/OrderService/Order.WebAPI"

RUN dotnet build "OrderService.WebAPI.csproj" -c Release -o /app
    
FROM build AS publish
RUN dotnet publish "OrderService.WebAPI.csproj" -c Release -o /app
    
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OrderService.WebAPI.dll"]