namespace ECommerce532.ViewModels;

public class ProductWithDetailsVM
{
    public Product? Product { get; set; } = null!;
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
    public IEnumerable<ProductSubImg> ProductSubImgs { get; set; } = new List<ProductSubImg>();
}
