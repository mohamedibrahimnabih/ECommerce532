using ECommerce532.Validations;
using System.ComponentModel.DataAnnotations;

namespace ECommerce532.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    [LetterOnly(3, 100)]
    public string Name { get; set; } = string.Empty;
    [Length(3, 100)]
    public string? Description { get; set; }
    public bool Status { get; set; }

    //public ICollection<Product> Products { get; } = new List<Product>();
}
