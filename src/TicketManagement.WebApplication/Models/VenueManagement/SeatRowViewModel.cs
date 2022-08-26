using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.VenueManagement
{
    public class SeatRowViewModel
    {
        public int AreaId { get; set; }

        public int Row { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Should be positive. Max value: 2147483647.")]
        [Display(Name = "Length")]
        public int Length { get; set; }
    }
}
