namespace ECommerce532.ViewModels;

public class ProductWithRelatedVM
{
    public Product Product { get; set; } = null!;
    public IEnumerable<Product> RelatedProducts { get; set; } = [];
}
