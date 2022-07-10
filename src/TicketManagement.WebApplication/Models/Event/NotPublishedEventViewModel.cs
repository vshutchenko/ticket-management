using TicketManagement.WebApplication.Models.EventArea;

namespace TicketManagement.WebApplication.Models.Event
{
    public class NotPublishedEventViewModel
    {
        public EventViewModel? Event { get; set; }

        public List<EventAreaViewModel> Areas { get; set; } = new List<EventAreaViewModel>();
    }
}
