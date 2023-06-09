namespace Public.DTO;

public class ProductExistenceData
{
    public Guid ProductId { get; set; }
    public string? Location { get; set; }
    public float Amount { get; set; }
}