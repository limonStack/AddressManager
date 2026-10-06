# AddressManager.Client — Angular Frontend

Angular 19 приложение для отображения адресов из базы данных через AddressManager.Api.

## Запуск

Стандартный способ — открыть `AddressManager.sln` в корне репозитория и нажать **F5**.
API запустится и автоматически поднимет Angular dev-сервер. Браузер откроется сам.

Если нужно запустить Angular отдельно (например, API уже запущен):
```powershell
cd src\AddressManager.Client
npm start
# Фронтенд: http://localhost:4200
```

## Структура проекта

```
src/
├── app/
│   ├── core/
│   │   ├── models/
│   │   │   └── address.model.ts          # Интерфейс Address
│   │   └── services/
│   │       └── address.service.ts        # HTTP-сервис для API
│   ├── features/
│   │   └── addresses/
│   │       ├── addresses.component.ts    # Компонент таблицы
│   │       ├── addresses.component.html  # Шаблон
│   │       └── addresses.component.scss  # Стили
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
└── environments/
    ├── environment.ts       # dev:  apiUrl = http://localhost:5017/api
    └── environment.prod.ts  # prod: apiUrl = /api
```

## Возможности UI

- Таблица с адресами из БД (страна, регион, город, улица, индекс)
- Флаги стран через Unicode emoji
- Сортировка по любому столбцу (клик по заголовку)
- Живой поиск по улице, городу, региону, стране, индексу
- Счётчик найденных записей
- Состояния: загрузка (spinner), ошибка, пустой результат поиска

## Сборка production

```powershell
npm run build
# Результат в dist/address-app/
```
