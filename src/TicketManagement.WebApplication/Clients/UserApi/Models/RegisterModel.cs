using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Clients.UserApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Display(Name = "Language")]
        public string? CultureName { get; set; }

        [Display(Name = "Time zone")]
        public string? TimeZoneId { get; set; }

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
