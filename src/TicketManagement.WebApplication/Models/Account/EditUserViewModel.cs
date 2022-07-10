using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketManagement.WebApplication.Models.Account
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

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

        public decimal Balance { get; set; }
    }
}
