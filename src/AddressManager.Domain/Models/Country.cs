namespace AddressManager.Domain.Models;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // ISO 3166-1 alpha-2

    public ICollection<Region> Regions { get; set; } = new List<Region>();
}
