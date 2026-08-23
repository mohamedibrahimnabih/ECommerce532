namespace ECommerce532.ViewModels;

public class BrandWithFilterVM
{
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();

    public string Query { get; set; } = string.Empty;
    public double TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
