using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Layout;
using TicketManagement.WebApplication.Models.Venue;

namespace TicketManagement.WebApplication.Models.Purchase
{
    public class PurchasedSeatViewModel
    {
        public int Id { get; set; }

        public EventViewModel? Event { get; set; }

        public EventAreaViewModel? Area { get; set; }

        public EventSeatViewModel? Seat { get; set; }

        public LayoutViewModel? Layout { get; set; }

        public VenueViewModel? Venue { get; set; }
    }
}
