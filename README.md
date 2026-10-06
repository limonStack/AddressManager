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
├── AddressManager.sln               # решение для Visual Studio
├── start-dev.bat                    # запуск одним кликом
└── README.md
```

## Запуск

### Вариант 1 — Visual Studio (рекомендуется)
1. Открыть `AddressManager.sln`.
2. Выбрать профиль **http** у `AddressManager.Api` и нажать **F5**.
3. API стартует в отладке и автоматически:
   - применяет миграции и создаёт БД (если её ещё нет)
   - устанавливает npm-зависимости (`npm ci`) при первом запуске
   - запускает Angular dev-сервер
   - открывает браузер, когда http://localhost:4200 станет доступен

При остановке отладки Angular-процесс также завершается автоматически.

### Вариант 2 — ручной запуск из терминала

Запустить API (в первом терминале):
```powershell
cd src\AddressManager.Api
dotnet run
```

API сам поднимет Angular dev-сервер. Если нужно запустить Angular отдельно — второй терминал:
```powershell
cd src\AddressManager.Client
npm start
```

## Миграции

Миграции применяются **автоматически** при каждом запуске API в режиме Development (`dotnet run` или F5). Вручную запускать `dotnet ef database update` не нужно.

Если нужно добавить новую миграцию при изменении моделей:
```powershell
cd src\AddressManager.Api
dotnet ef migrations add <НазваниеМиграции> --project ..\AddressManager.Domain\AddressManager.Domain.csproj --startup-project AddressManager.Api.csproj
```

После этого просто запусти API — миграция применится автоматически.

## Порты

| Сервис  | URL                    |
|---------|------------------------|
| API     | http://localhost:5017  |
| Angular | http://localhost:4200  |
| DB      | (localdb)\mssqllocaldb |
