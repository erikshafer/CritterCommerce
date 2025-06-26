namespace Inventory.Api.Locations;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; } = null!;
}
