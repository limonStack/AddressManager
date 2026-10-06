using AddressManager.Domain.Data;
using AddressManager.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS — разрешаем Angular dev-сервер
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AddressDb")));

// Автозапуск Angular dev-сервера в режиме разработки
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<AngularDevServerService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // F5 должен подготовить локальную БД без отдельной команды dotnet ef.
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    app.MapOpenApi();
}

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
