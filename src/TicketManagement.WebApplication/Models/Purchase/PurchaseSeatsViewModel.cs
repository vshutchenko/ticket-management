using System.ComponentModel.DataAnnotations;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;

namespace TicketManagement.WebApplication.Models.Purchase
{
    public class PurchaseSeatsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        public int LayoutId { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public string ImageUrl { get; set; } = string.Empty;

        public List<EventAreaViewModel> Areas { get; set; } = new List<EventAreaViewModel>();

        public List<EventSeatViewModel> Seats { get; set; } = new List<EventSeatViewModel>();
    }
}
