namespace ECommerce532.ViewModels;

public class ProductWithFilterVM
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();

    public string Name { get; set; } = string.Empty;
    public decimal? MinPrice { get; set; } 
    public decimal? MaxPrice { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? LessQuantity { get; set; }
    public double TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
