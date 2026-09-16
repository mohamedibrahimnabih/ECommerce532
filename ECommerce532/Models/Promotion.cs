namespace ECommerce532.Models;

public class Promotion : Audit
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public decimal Discount { get; set; }
    public int MaxUsage { get; set; } = 100;
    public DateTime ValidTo { get; set; } = DateTime.Now.AddMonths(1);

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public bool Status { get; set; }
}
