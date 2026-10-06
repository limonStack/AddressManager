using AddressManager.Domain.DTOs;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// Контракт репозитория адресов.
///
/// Интерфейс живёт в Domain — слое, который не зависит ни от API, ни от EF Core.
/// Благодаря этому контроллер зависит только от абстракции, а не от конкретной реализации.
/// Это позволяет легко подменить реализацию (например, сменить SQL Server на DynamoDB)
/// или подставить мок в тестах.
/// </summary>
public interface IAddressRepository
{
    /// <summary>
    /// Возвращает все адреса, отсортированные по стране → городу → улице.
    /// </summary>
    Task<IEnumerable<AddressDto>> GetAllAsync();

    /// <summary>
    /// Возвращает адрес по идентификатору.
    /// Возвращает null, если адрес с таким id не найден.
    /// </summary>
    Task<AddressDto?> GetByIdAsync(int id);
}
