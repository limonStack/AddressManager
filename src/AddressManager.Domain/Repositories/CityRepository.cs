using AddressManager.Domain.Data;
using AddressManager.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// Реализация репозитория городов на основе EF Core.
/// </summary>
public sealed class CityRepository : ICityRepository
{
    private readonly AppDbContext _db;

    public CityRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<IEnumerable<CityDto>> GetAllAsync()
    {
        return await _db.Cities
            .Include(c => c.Region)
                .ThenInclude(r => r.Country)
            .Select(c => new CityDto
            {
                Id      = c.Id,
                Name    = c.Name,
                Region  = c.Region.Name,
                Country = c.Region.Country.Name
            })
            .OrderBy(c => c.Country)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }
}
