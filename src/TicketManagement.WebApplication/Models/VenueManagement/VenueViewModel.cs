using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.VenueManagement
{
    public class VenueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        public List<LayoutViewModel> Layouts { get; set; } = new List<LayoutViewModel>();
    }
}
