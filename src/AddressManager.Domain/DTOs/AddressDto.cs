namespace AddressManager.Domain.DTOs;

/// <summary>
/// DTO (Data Transfer Object) для передачи данных адреса из репозитория в контроллер.
///
/// Содержит плоскую структуру: вместо вложенных объектов City → Region → Country
/// используются строковые имена. Это удобно для сериализации в JSON и отображения в Angular.
/// </summary>
public sealed class AddressDto
{
    public int    Id              { get; init; }
    public string Street         { get; init; } = string.Empty;
    public string HouseNumber    { get; init; } = string.Empty;

    /// <summary>Номер квартиры. null, если адрес без квартиры.</summary>
    public string? ApartmentNumber { get; init; }

    public string PostalCode    { get; init; } = string.Empty;
    public string City          { get; init; } = string.Empty;
    public string Region        { get; init; } = string.Empty;
    public string Country       { get; init; } = string.Empty;

    /// <summary>ISO 3166-1 alpha-2 код страны, например "UA", "PL", "DE".</summary>
    public string CountryCode   { get; init; } = string.Empty;
}
