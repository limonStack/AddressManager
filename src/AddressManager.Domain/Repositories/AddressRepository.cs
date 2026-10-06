using AddressManager.Domain.Data;
using AddressManager.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// Реализация репозитория адресов на основе Entity Framework Core.
///
/// Вся логика работы с БД сосредоточена здесь: Include, ThenInclude, Select, OrderBy.
/// Контроллер об этом не знает — он работает только через IAddressRepository.
/// </summary>
public sealed class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _db;

    public AddressRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<IEnumerable<AddressDto>> GetAllAsync()
    {
        return await _db.Addresses
            // Загружаем всю географическую цепочку: Адрес → Город → Регион → Страна
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
            // Проецируем в DTO — возвращаем плоский объект вместо графа EF-сущностей
            .Select(a => new AddressDto
            {
                Id              = a.Id,
                Street          = a.Street,
                HouseNumber     = a.HouseNumber,
                ApartmentNumber = a.ApartmentNumber,
                PostalCode      = a.PostalCode,
                City            = a.City.Name,
                Region          = a.City.Region.Name,
                Country         = a.City.Region.Country.Name,
                CountryCode     = a.City.Region.Country.Code
            })
            .OrderBy(a => a.Country)
            .ThenBy(a => a.City)
            .ThenBy(a => a.Street)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<AddressDto?> GetByIdAsync(int id)
    {
        return await _db.Addresses
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
            .Where(a => a.Id == id)
            .Select(a => new AddressDto
            {
                Id              = a.Id,
                Street          = a.Street,
                HouseNumber     = a.HouseNumber,
                ApartmentNumber = a.ApartmentNumber,
                PostalCode      = a.PostalCode,
                City            = a.City.Name,
                Region          = a.City.Region.Name,
                Country         = a.City.Region.Country.Name,
                CountryCode     = a.City.Region.Country.Code
            })
            .FirstOrDefaultAsync(); // null если не найден
    }
}
