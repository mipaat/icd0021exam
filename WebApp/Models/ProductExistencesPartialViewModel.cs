using Domain;

namespace WebApp.Models;

public class ProductExistencesPartialViewModel
{
    public List<Product> Products { get; set; } = default!;

    public ProductExistencesPartialViewModel()
    {
    }

    public ProductExistencesPartialViewModel(IEnumerable<Product> products)
    {
        Products = products.ToList();
    }
    
    public ProductExistencesPartialViewModel(Product product)
    {
        Products = new List<Product> { product };
    }
}