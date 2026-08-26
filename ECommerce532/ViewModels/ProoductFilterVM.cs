namespace ECommerce532.ViewModels;

public record ProductFilterVM(string? name, decimal? minPrice, decimal? maxPrice, bool? lessQuantity, int? categoryId, int? brandId);
