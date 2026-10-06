namespace AddressManager.Domain.Models;

/// <summary>
/// Регион — второй уровень географической иерархии (область, воеводство, земля).
/// Примеры: "Киевская область", "Мазовецкое воеводство", "Бавария".
/// Каждый регион принадлежит одной стране.
/// </summary>
public class Region
{
    /// <summary>Первичный ключ, автоинкремент.</summary>
    public int Id { get; set; }

    /// <summary>Название региона.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Внешний ключ → Country.Id.</summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Навигационное свойство: страна, к которой принадлежит регион.
    /// null! — означает, что свойство никогда не будет null после загрузки из БД
    /// (EF Core гарантирует это при корректном Include).
    /// </summary>
    public Country Country { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: города этого региона.
    /// </summary>
    public ICollection<City> Cities { get; set; } = new List<City>();
}
