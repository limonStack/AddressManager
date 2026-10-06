using AddressManager.Domain.Commands;
using AddressManager.Domain.Data;
using AddressManager.Domain.DTOs;
using AddressManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// Реализация Command-репозитория адресов.
/// Отвечает исключительно за запись.
///
/// После сохранения перечитывает созданный адрес с навигационными свойствами,
/// чтобы вернуть полный AddressDto — клиент сразу получает строку для таблицы.
/// </summary>
public sealed class AddressCommandRepository : IAddressCommandRepository
{
    private readonly AppDbContext _db;

    public AddressCommandRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<AddressDto> CreateAsync(CreateAddressCommand command)
    {
        // Создаём сущность из команды — только нормализованные данные (CityId)
        var address = new Address
        {
            Street          = command.Street,
            HouseNumber     = command.HouseNumber,
            ApartmentNumber = command.ApartmentNumber,
            PostalCode      = command.PostalCode,
            CityId          = command.CityId
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        // После сохранения перечитываем с Include, чтобы получить полную иерархию
        // и вернуть AddressDto — модель чтения, а не модель записи
        return await _db.Addresses
            .Include(a => a.City)
                .ThenInclude(c => c.Region)
                    .ThenInclude(r => r.Country)
            .Where(a => a.Id == address.Id)
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
            .FirstAsync();
    }
}
