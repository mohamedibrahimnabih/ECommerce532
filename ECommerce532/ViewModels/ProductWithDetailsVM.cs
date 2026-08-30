using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce532.ViewModels;

public class ProductWithDetailsVM
{
    public Product? Product { get; set; } = null!;
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
    public IEnumerable<ProductSubImg> ProductSubImgs { get; set; } = new List<ProductSubImg>();
}
