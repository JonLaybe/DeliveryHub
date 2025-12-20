# Infrastructure

Инфраструктура **окружения**, необходимая для работы всей системы.

Папка отвечает на вопрос:
"Какие внешние компоненты нужны системе и как их запустить?"

## Что здесь находится
- Docker / docker-compose для локальной разработки
- Конфигурации MongoDB, RabbitMQ и других сервисов
- Init-скрипты, volumes, конфиги
- Observability (логирование, мониторинг)

## Структура
- Docker/ — общие docker-compose файлы
- Mongo/ — запуск и инициализация MongoDB
- Rabbit/ — запуск и конфигурация RabbitMQ

## Чего здесь НЕ должно быть
- Код сервисов
- Repository, DAL, DbContext
- Kubernetes manifests (они в Deploy)

## Принцип
Infrastructure = *как запустить окружение*
