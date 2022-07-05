using TicketManagement.WebApplication.Models.Area;

namespace TicketManagement.WebApplication.Models.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int LayoutId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } = DateTime.Now;

        public string? ImageUrl { get; set; }

        public bool Published { get; set; }
    }
}
