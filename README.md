# AddressManager

Full-stack приложение: ASP.NET Core 10 API + Angular 19 + SQL Server LocalDB.

## Структура

```
AddressManager/
├── src/
│   ├── AddressManager.Api/          # ASP.NET Core Web API (порт 5017)
│   │   ├── Controllers/
│   │   ├── Infrastructure/
│   │   │   └── AngularDevServerService.cs  # автозапуск фронта
│   │   ├── Properties/launchSettings.json
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── AddressManager.Domain/       # EF Core: модели, DbContext, миграции
│   │   ├── Data/AppDbContext.cs
│   │   ├── Models/
│   │   └── Migrations/
│   └── AddressManager.Client/       # Angular 19 (порт 4200)
│       └── src/
├── AddressManager.sln
├── start-dev.bat                    # запуск одним кликом
└── README.md
```

## Запуск одним кликом

### Вариант 1 — двойной клик на `start-dev.bat`
Запускает API, который автоматически поднимет Angular dev-сервер.

### Вариант 2 — Visual Studio
1. Открыть `AddressManager.sln`
2. Нажать **F5** (профиль `http`)
3. API стартует → автоматически запускает `npm start` в фоне → открывает браузер на http://localhost:4200

## Первый запуск — миграции

```powershell
cd src\AddressManager.Api

# Установить dotnet-ef (один раз)
dotnet tool install --global dotnet-ef

# Применить миграции (создаст БД в LocalDB)
dotnet ef database update --project ..\AddressManager.Domain\AddressManager.Domain.csproj --startup-project AddressManager.Api.csproj
```

## Новая миграция при изменении моделей

```powershell
cd src\AddressManager.Api
dotnet ef migrations add <НазваниеМиграции> --project ..\AddressManager.Domain\AddressManager.Domain.csproj --startup-project AddressManager.Api.csproj
dotnet ef database update --project ..\AddressManager.Domain\AddressManager.Domain.csproj --startup-project AddressManager.Api.csproj
```

## Порты

| Сервис  | URL                          |
|---------|------------------------------|
| API     | http://localhost:5017        |
| Angular | http://localhost:4200        |
| DB      | (localdb)\mssqllocaldb       |
