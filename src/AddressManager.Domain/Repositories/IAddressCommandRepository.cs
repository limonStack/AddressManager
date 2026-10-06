using AddressManager.Domain.Commands;
using AddressManager.Domain.DTOs;

namespace AddressManager.Domain.Repositories;

/// <summary>
/// CQRS — Command-сторона: только операции записи адресов.
///
/// Принимает CreateAddressCommand — нормализованную модель с CityId,
/// возвращает AddressDto созданного адреса (чтобы клиент мог сразу добавить
/// запись в таблицу без повторного запроса).
/// </summary>
public interface IAddressCommandRepository
{
    /// <summary>
    /// Создаёт новый адрес и возвращает его полное представление (AddressDto)
    /// с развёрнутой географической иерархией.
    /// </summary>
    Task<AddressDto> CreateAsync(CreateAddressCommand command);
}
