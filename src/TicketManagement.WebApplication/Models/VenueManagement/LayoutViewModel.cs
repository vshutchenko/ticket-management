using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.VenueManagement
{
    public class LayoutViewModel
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
