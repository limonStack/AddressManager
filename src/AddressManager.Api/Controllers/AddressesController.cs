// ============================================================
// AddressesController — REST API для работы с адресами
//
// Маршрут: /api/addresses
//
// Доступные эндпоинты:
//   GET /api/addresses        — список всех адресов
//   GET /api/addresses/{id}   — один адрес по идентификатору
//
// Контроллер намеренно тонкий: он не знает об EF Core, SQL или структуре БД.
// Вся логика работы с данными инкапсулирована в IAddressRepository.
// ============================================================

using AddressManager.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AddressManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    // Зависимость от абстракции, а не от конкретной реализации.
    // Это позволяет подменить реализацию в тестах (mock) или при смене БД.
    private readonly IAddressRepository _repo;

    public AddressesController(IAddressRepository repo) => _repo = repo;

    /// <summary>
    /// Возвращает список всех адресов, отсортированных по стране → городу → улице.
    /// </summary>
    // GET api/addresses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var addresses = await _repo.GetAllAsync();
        return Ok(addresses);
    }

    /// <summary>
    /// Возвращает один адрес по числовому идентификатору.
    /// Если адрес не найден — 404 Not Found.
    /// </summary>
    // GET api/addresses/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var address = await _repo.GetByIdAsync(id);
        return address is null ? NotFound() : Ok(address);
    }
}
