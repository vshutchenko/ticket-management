using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.EventArea
{
    public class EventAreaViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }
        [Range(0.01, 100000000, ErrorMessage = "Price must be greater than zero!")]
        public decimal Price { get; set; }
    }
}
