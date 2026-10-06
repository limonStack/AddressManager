using AddressManager.Domain.DTOs;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// CQRS — Query-сторона: только операции чтения адресов.
///
/// Возвращает AddressDto — денормализованную плоскую модель,
/// удобную для отображения в таблице на клиенте.
/// </summary>
public interface IAddressQueryRepository
{
    /// <summary>Все адреса, отсортированные по стране → городу → улице.</summary>
    Task<IEnumerable<AddressDto>> GetAllAsync();

    /// <summary>Один адрес по Id. null — если не найден.</summary>
    Task<AddressDto?> GetByIdAsync(int id);
}
