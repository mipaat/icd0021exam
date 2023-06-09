using Base.Domain;

namespace Domain;

public class ProductCategory : AbstractIdDatabaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}