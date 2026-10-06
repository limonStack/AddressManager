// ============================================================
// CitiesController — REST API для получения списка городов
//
// Используется Angular-формой создания адреса:
//   1. Форма загружает GET /api/cities
//   2. Пользователь выбирает город из выпадающего списка
//   3. CityId отправляется в POST /api/addresses (CreateAddressCommand)
// ============================================================

using AddressManager.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AddressManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityRepository _repo;

    public CitiesController(ICityRepository repo) => _repo = repo;

    /// <summary>GET /api/cities — все города с регионом и страной.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllAsync());
}
