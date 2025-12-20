# Services

Микросервисы системы DeliveryHub.

Каждый сервис является логически и технически изолированным
и может быть вынесен в отдельный репозиторий.

## Структура
Каждый сервис располагается в собственной папке и содержит:
- API слой
- Application слой
- Domain слой
- Infrastructure слой
- Отдельный solution (.sln)

## Пример
Services/
  └─ CatalogService/
     ├─ Catalog.Api
     ├─ Catalog.Application
     ├─ Catalog.Domain
     ├─ Catalog.Infrastructure
     └─ Catalog.sln

## Принцип
Один сервис — одна бизнес-область