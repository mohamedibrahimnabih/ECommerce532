using System.ComponentModel.DataAnnotations;

namespace ECommerce532.ViewModels;

public class LoginVM
{
    [Required]
    [Display(Name = "Email Or UserName")]
    public string EmailOrUserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool Remember { get; set; }
}
