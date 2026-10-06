using AddressManager.Domain.DTOs;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// Репозиторий городов — только чтение.
/// Используется для заполнения выпадающего списка в форме создания адреса.
/// </summary>
public interface ICityRepository
{
    /// <summary>Все города с названием региона и страны, отсортированные по стране → городу.</summary>
    Task<IEnumerable<CityDto>> GetAllAsync();
}
