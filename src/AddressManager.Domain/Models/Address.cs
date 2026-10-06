namespace AddressManager.Domain.Models;

/// <summary>
/// Адрес — нижний уровень географической иерархии.
/// Содержит конкретный почтовый адрес: улица, дом, квартира, индекс.
/// Каждый адрес принадлежит одному городу.
/// </summary>
public class Address
{
    /// <summary>Первичный ключ, автоинкремент.</summary>
    public int Id { get; set; }

    /// <summary>Название улицы, проспекта и т.д. Например: "вул. Хрещатик".</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Номер дома. Строка, так как может содержать литеры: "12А", "64/2".</summary>
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Номер квартиры. Nullable — частные дома и офисы не имеют квартиры.
    /// </summary>
    public string? ApartmentNumber { get; set; }

    /// <summary>Почтовый индекс. Строка, так как формат отличается по странам: "01001", "00-026", "80539".</summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>Внешний ключ → City.Id.</summary>
    public int CityId { get; set; }

    /// <summary>
    /// Навигационное свойство: город, в котором находится адрес.
    /// null! — EF Core заполняет при Include().
    /// </summary>
    public City City { get; set; } = null!;
}
