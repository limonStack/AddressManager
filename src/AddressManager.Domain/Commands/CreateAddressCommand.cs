namespace AddressManager.Domain.Commands;

/// <summary>
/// Команда создания нового адреса (модель записи в CQRS).
///
/// Намеренно отличается от AddressDto (модели чтения):
/// - Принимает CityId (внешний ключ), а не строковые City/Region/Country
/// - Клиент выбирает город из списка и отправляет только его Id
/// - Сервер сам разворачивает иерархию (город → регион → страна) при сохранении
///
/// Это и есть суть CQRS: модель записи оптимизирована под операцию записи,
/// модель чтения (AddressDto) — под операцию чтения.
/// </summary>
public sealed class CreateAddressCommand
{
    /// <summary>Название улицы. Обязательное поле.</summary>
    public string Street { get; init; } = string.Empty;

    /// <summary>Номер дома. Обязательное поле.</summary>
    public string HouseNumber { get; init; } = string.Empty;

    /// <summary>Номер квартиры. Необязательное — null для частных домов.</summary>
    public string? ApartmentNumber { get; init; }

    /// <summary>Почтовый индекс. Обязательное поле.</summary>
    public string PostalCode { get; init; } = string.Empty;

    /// <summary>
    /// Id города из таблицы Cities.
    /// Клиент получает список городов через GET /api/cities и передаёт выбранный Id.
    /// </summary>
    public int CityId { get; init; }
}
