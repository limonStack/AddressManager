namespace AddressManager.Domain.Models;

/// <summary>
/// Страна — верхний уровень географической иерархии.
/// Пример: Украина (UA), Польша (PL), Германия (DE).
/// </summary>
public class Country
{
    /// <summary>Первичный ключ, автоинкремент.</summary>
    public int Id { get; set; }

    /// <summary>Полное название страны, например "Украина".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Двухбуквенный код страны по стандарту ISO 3166-1 alpha-2.
    /// Используется для отображения флага-эмодзи на клиенте.
    /// Примеры: "UA", "PL", "DE".
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Навигационное свойство: регионы, принадлежащие этой стране.
    /// Заполняется EF Core при использовании Include() или явной загрузки.
    /// </summary>
    public ICollection<Region> Regions { get; set; } = new List<Region>();
}
