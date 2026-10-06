namespace AddressManager.Domain.DTOs;

/// <summary>
/// DTO города для выпадающего списка в форме создания адреса.
/// Содержит Id (для отправки в команде) и отображаемое название с регионом и страной.
/// </summary>
public sealed class CityDto
{
    public int    Id          { get; init; }
    public string Name        { get; init; } = string.Empty;
    public string Region      { get; init; } = string.Empty;
    public string Country     { get; init; } = string.Empty;

    /// <summary>Удобное отображение: "Киев (Киевская область, Украина)".</summary>
    public string DisplayName => $"{Name} ({Region}, {Country})";
}
