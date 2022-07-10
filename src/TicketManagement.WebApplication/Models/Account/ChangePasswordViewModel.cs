using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [Compare("NewPassword", ErrorMessage = "Entered passwords don't match!")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
