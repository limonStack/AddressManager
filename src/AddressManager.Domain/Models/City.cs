namespace AddressManager.Domain.Models;

/// <summary>
/// Город — третий уровень географической иерархии.
/// Примеры: Киев, Варшава, Мюнхен.
/// Каждый город принадлежит одному региону.
/// </summary>
public class City
{
    /// <summary>Первичный ключ, автоинкремент.</summary>
    public int Id { get; set; }

    /// <summary>Название города.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Внешний ключ → Region.Id.</summary>
    public int RegionId { get; set; }

    /// <summary>
    /// Навигационное свойство: регион, в котором находится город.
    /// null! — EF Core заполняет при Include().
    /// </summary>
    public Region Region { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: адреса, находящиеся в этом городе.
    /// </summary>
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
