using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Clients.UserApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
