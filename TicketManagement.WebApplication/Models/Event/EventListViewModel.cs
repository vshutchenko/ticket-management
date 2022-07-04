namespace TicketManagement.WebApplication.Models.Event
{
    public class EventListViewModel
    {
        public EventListViewModel(IEnumerable<EventViewModel> events, PagingInfo pagingInfo)
        {
            Events = events ?? throw new ArgumentNullException(nameof(events));
            PagingInfo = pagingInfo ?? throw new ArgumentNullException(nameof(pagingInfo));
        }

        public IEnumerable<EventViewModel> Events { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
