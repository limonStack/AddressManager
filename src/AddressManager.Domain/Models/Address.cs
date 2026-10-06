namespace AddressManager.Domain.Models;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public string PostalCode { get; set; } = string.Empty;

    public int CityId { get; set; }
    public City City { get; set; } = null!;
}
