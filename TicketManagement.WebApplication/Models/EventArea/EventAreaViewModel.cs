using System.ComponentModel.DataAnnotations;
using TicketManagement.WebApplication.Models.EventSeat;

namespace TicketManagement.WebApplication.Models.EventArea
{
    public class EventAreaViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        public int CoordX { get; set; }

        public int CoordY { get; set; }
        [Range(0.01, 100000000, ErrorMessage = "Price must be greter than zero !")]
        public decimal Price { get; set; }
    }
}
