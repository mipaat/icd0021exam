using Base.Domain;

namespace Public.DTO;

public class Product : AbstractIdDatabaseEntity
{
    public string Name { get; set; } = default!;
    public string? Unit { get; set; }
}