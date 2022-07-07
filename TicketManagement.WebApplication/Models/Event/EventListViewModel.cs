namespace TicketManagement.WebApplication.Models.Event
{
    public class EventListViewModel
    {
        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
        public PagingInfo? PagingInfo { get; set; }
    }
}
