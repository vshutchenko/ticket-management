using TicketManagement.WebApplication.Models.EventSeat;

namespace TicketManagement.WebApplication.Models.EventArea
{
    public class EventAreaViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string? Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public decimal Price { get; set; }
    }
}
