// ============================================================
// AddressesController — REST API для работы с адресами
//
// CQRS в действии:
//   GET-запросы  → IAddressQueryRepository   (чтение)
//   POST-запросы → IAddressCommandRepository (запись)
//
// Контроллер не знает об EF Core, SQL и структуре БД.
// ============================================================

using AddressManager.Domain.Commands;
using AddressManager.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AddressManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IAddressQueryRepository   _query;
    private readonly IAddressCommandRepository _command;

    public AddressesController(
        IAddressQueryRepository   query,
        IAddressCommandRepository command)
    {
        _query   = query;
        _command = command;
    }

    /// <summary>GET /api/addresses — все адреса.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _query.GetAllAsync());

    /// <summary>GET /api/addresses/5 — адрес по Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var address = await _query.GetByIdAsync(id);
        return address is null ? NotFound() : Ok(address);
    }

    /// <summary>
    /// POST /api/addresses — создать новый адрес.
    /// Принимает CreateAddressCommand (модель записи),
    /// возвращает AddressDto созданного адреса (модель чтения) со статусом 201 Created.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressCommand command)
    {
        var created = await _command.CreateAsync(command);

        // 201 Created + Location: /api/addresses/{id} + тело ответа
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
