// ============================================================
// AddressesController — REST API для работы с адресами
//
// Маршрут: /api/addresses
//
// Доступные эндпоинты:
//   GET /api/addresses        — список всех адресов
//   GET /api/addresses/{id}   — один адрес по идентификатору
// ============================================================

using AddressManager.Domain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Api.Controllers;

// [ApiController] — включает автоматическую валидацию модели, привязку параметров из JSON,
//                   и стандартные ответы 400 Bad Request при ошибках модели.
// [Route("api/[controller]")] — [controller] подставляется как "addresses" (имя класса без суффикса Controller)
[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    // Контекст базы данных — внедряется через DI (зарегистрирован в Program.cs)
    private readonly AppDbContext _db;

    // Конструктор с инъекцией зависимости — короткая форма присвоения поля
    public AddressesController(AppDbContext db) => _db = db;

    /// <summary>
    /// Возвращает список всех адресов с полной географической иерархией:
    /// Адрес → Город → Регион → Страна.
    /// Результат отсортирован: сначала по стране, затем по городу, затем по улице.
    /// </summary>
    // GET api/addresses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var addresses = await _db.Addresses
            // Include + ThenInclude загружают связанные сущности через JOIN.
            // Без этого навигационные свойства (City, Region, Country) были бы null.
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
            // Select проецирует EF-сущности в анонимный DTO.
            // Это позволяет вернуть плоский объект вместо вложенного графа сущностей,
            // что удобнее для Angular и безопаснее (не утекают внутренние поля).
            .Select(a => new
            {
                a.Id,
                a.Street,
                a.HouseNumber,
                a.ApartmentNumber,
                a.PostalCode,
                City        = a.City.Name,
                Region      = a.City.Region.Name,
                Country     = a.City.Region.Country.Name,
                CountryCode = a.City.Region.Country.Code   // ISO 3166-1 alpha-2 (например "UA")
            })
            .OrderBy(a => a.Country)
            .ThenBy(a => a.City)
            .ThenBy(a => a.Street)
            .ToListAsync(); // выполняем SQL-запрос асинхронно

        return Ok(addresses); // 200 OK + JSON-массив
    }

    /// <summary>
    /// Возвращает один адрес по числовому идентификатору.
    /// Если адрес не найден — 404 Not Found.
    /// </summary>
    // GET api/addresses/5
    [HttpGet("{id:int}")] // {id:int} — маршрутное ограничение: принимаем только целые числа
    public async Task<IActionResult> GetById(int id)
    {
        var address = await _db.Addresses
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.Street,
                a.HouseNumber,
                a.ApartmentNumber,
                a.PostalCode,
                City        = a.City.Name,
                Region      = a.City.Region.Name,
                Country     = a.City.Region.Country.Name,
                CountryCode = a.City.Region.Country.Code
            })
            .FirstOrDefaultAsync(); // null, если запись не найдена

        // Паттерн: null → 404 Not Found, иначе → 200 OK + объект
        return address is null ? NotFound() : Ok(address);
    }
}
