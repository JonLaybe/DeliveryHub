FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
    
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Order.WebAPI/OrderService.WebAPI.csproj"
WORKDIR "/src/Order.WebAPI"
RUN dotnet build "OrderService.WebAPI.csproj" -c Release -o /app
    
FROM build AS publish
WORKDIR "/src/Order.WebAPI"
RUN dotnet publish "OrderService.WebAPI.csproj" -c Release -o /app
    
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OrderService.WebAPI.dll"]