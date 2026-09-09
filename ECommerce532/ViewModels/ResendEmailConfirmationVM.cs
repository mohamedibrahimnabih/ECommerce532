using System.ComponentModel.DataAnnotations;

namespace ECommerce532.ViewModels;

public class ResendEmailConfirmationVM
{
    [Required]
    [Display(Name = "Email Or UserName")]
    public string EmailOrUserName { get; set; } = string.Empty;
}
