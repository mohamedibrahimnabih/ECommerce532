namespace ECommerce532.Models;

public class Audit
{
    public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateAt { get; set; }

    //public string? CreatedById { get; set; }
    //public ApplicationUser? CreatedBy { get; set; }

    //public string? UpdateById { get; set; }
    //public ApplicationUser? UpdateBy { get; set; }
}
