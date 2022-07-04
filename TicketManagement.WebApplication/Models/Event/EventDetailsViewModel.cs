using System.ComponentModel.DataAnnotations;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;

namespace TicketManagement.WebApplication.Models.Event
{
    public class EventDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        public int LayoutId { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public List<EventAreaViewModel> Areas { get; set; } = new List<EventAreaViewModel>();
        public List<EventSeatViewModel> Seats { get; set; } = new List<EventSeatViewModel>();
    }
}
