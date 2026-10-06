// ============================================================
// Точка входа приложения AddressManager API
//
// Здесь происходит:
//   1. Регистрация всех сервисов в DI-контейнере (builder.Services)
//   2. Построение приложения (app = builder.Build())
//   3. Настройка middleware-пайплайна
//   4. Запуск сервера (app.Run())
// ============================================================

using AddressManager.Domain.Data;
using AddressManager.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем MVC-контроллеры (AddressesController и любые будущие)
builder.Services.AddControllers();

// Включаем генерацию OpenAPI / Swagger-документации
builder.Services.AddOpenApi();

// CORS — разрешаем запросы с Angular dev-сервера (http://localhost:4200).
// Без этой политики браузер блокирует fetch-запросы из Angular к API,
// потому что порты 4200 и 5017 разные (cross-origin).
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Подключаем Entity Framework Core с провайдером SQL Server.
// Строка подключения берётся из appsettings.json → "ConnectionStrings:AddressDb".
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AddressDb")));

// В режиме разработки автоматически запускаем Angular dev-сервер
// как фоновый IHostedService, чтобы не запускать его вручную.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<AngularDevServerService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Автоматически применяем все pending-миграции при запуске (F5).
    // Это избавляет от необходимости каждый раз вручную вызывать
    // `dotnet ef database update` перед первым запуском.
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // Подключаем эндпоинт /openapi/v1.json для Swagger UI
    app.MapOpenApi();
}

// Применяем CORS-политику до обработчиков запросов.
// Порядок middleware важен: CORS должен идти раньше Authorization и Controllers.
app.UseCors("AllowAngular");

// Включаем проверку авторизации (сейчас контроллеры не защищены,
// но middleware нужен для корректной работы атрибутов [Authorize] в будущем)
app.UseAuthorization();

// Регистрируем маршруты всех контроллеров (например, GET /api/addresses)
app.MapControllers();

app.Run();
