using Base.Domain;
using Public.DTO.Identity;

namespace Public.DTO;

public class ProductExistence : AbstractIdDatabaseEntity
{
    public Product Product { get; set; } = default!;
    public User User { get; set; } = default!;

    public string? Location { get; set; }
    public float Amount { get; set; }
}