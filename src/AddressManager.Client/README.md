# AddressApp — Angular Frontend

Angular 19 приложение для отображения адресов из базы данных через AddressApi.

## Структура проекта

```
src/
├── app/
│   ├── core/
│   │   ├── models/
│   │   │   └── address.model.ts      # Интерфейс Address
│   │   └── services/
│   │       └── address.service.ts    # HTTP-сервис для API
│   ├── features/
│   │   └── addresses/
│   │       ├── addresses.component.ts    # Компонент таблицы
│   │       ├── addresses.component.html  # Шаблон
│   │       └── addresses.component.scss  # Стили
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
└── environments/
    ├── environment.ts       # dev: http://localhost:5000/api
    └── environment.prod.ts  # prod: /api
```

## Запуск

### 1. Запустить API (AddressApi)

```powershell
cd e:\Work\Projects\AddressApi
dotnet run
# API будет доступен на http://localhost:5000
```

### 2. Запустить Angular dev-сервер

Открыть новый терминал (обязательно после перезапуска — чтобы Node.js был в PATH):

```powershell
cd e:\Work\Projects\AddressApp
npm start
# Фронтенд: http://localhost:4200
```

## Возможности UI

- Таблица с 10 адресами из БД (страна, регион, город, улица, индекс)
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
