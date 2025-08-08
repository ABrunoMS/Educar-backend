namespace Educar.Backend.Domain.Entities;

public class Address(
    string street,
    string city,
    string state,
    string postalCode,
    string country)
    : BaseAuditableEntity
{
    public string Street { get; set; } = street;
    public string City { get; set; } = city;
    public string State { get; set; } = state;
    public string PostalCode { get; set; } = postalCode;
    public string Country { get; set; } = country;
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
}