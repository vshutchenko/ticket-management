using TicketManagement.WebApplication.Models.Venue;

namespace TicketManagement.WebApplication.Models.Layout
{
    public class LayoutViewModel
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        public string? Description { get; set; }
    }
}
