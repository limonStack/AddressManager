using AddressManager.Domain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly AppDbContext _db;

    public AddressesController(AppDbContext db) => _db = db;

    // GET api/addresses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var addresses = await _db.Addresses
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
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
            .OrderBy(a => a.Country)
            .ThenBy(a => a.City)
            .ThenBy(a => a.Street)
            .ToListAsync();

        return Ok(addresses);
    }

    // GET api/addresses/5
    [HttpGet("{id:int}")]
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
            .FirstOrDefaultAsync();

        return address is null ? NotFound() : Ok(address);
    }
}
