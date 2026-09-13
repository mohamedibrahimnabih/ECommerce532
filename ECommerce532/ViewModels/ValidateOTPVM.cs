using System.ComponentModel.DataAnnotations;

namespace ECommerce532.ViewModels;

public class ValidateOTPVM
{
    [Required]
    public string OTP { get; set; } = string.Empty;
}
