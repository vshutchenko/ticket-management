using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketManagement.WebApplication.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Language")]
        public string CultureName { get; set; } = string.Empty;

        public List<SelectListItem> Cultures { get; set; } = new List<SelectListItem>();

        [Display(Name = "Time zone")]
        public string TimeZoneId { get; set; } = string.Empty;

        public List<SelectListItem> TimeZones { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "Entered passwords don't match!")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must contain at leats 5 characters.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
