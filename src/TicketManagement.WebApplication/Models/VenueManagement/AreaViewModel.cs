using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.VenueManagement
{
    public class AreaViewModel
    {
        public int Id { get; set; }

        public int LayoutId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Coordinate X is required")]
        [Display(Name = "Coordinate X")]
        public int CoordX { get; set; }

        [Required(ErrorMessage = "Coordinate Y is required")]
        [Display(Name = "Coordinate Y")]
        public int CoordY { get; set; }

        public List<SeatViewModel> Seats { get; set; } = new List<SeatViewModel>();
    }
}
