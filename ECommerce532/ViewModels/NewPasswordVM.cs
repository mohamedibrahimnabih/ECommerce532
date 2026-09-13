using System.ComponentModel.DataAnnotations;

namespace ECommerce532.ViewModels;

public class NewPasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
