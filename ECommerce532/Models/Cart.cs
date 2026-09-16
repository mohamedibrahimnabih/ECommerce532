namespace ECommerce532.Models;

public class Cart
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Count { get; set; }
    public decimal CurrentPrice { get; set; }
}
