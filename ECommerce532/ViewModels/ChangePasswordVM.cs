using System.ComponentModel.DataAnnotations;

namespace ECommerce532.ViewModels;

public class ChangePasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
