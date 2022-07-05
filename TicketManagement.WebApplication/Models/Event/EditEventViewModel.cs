using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.WebApplication.Models.EventArea;

namespace TicketManagement.WebApplication.Models.Event
{
    public class EditEventViewModel
    {
        public CreateEventViewModel? Event { get; set; }
        public List<EventAreaViewModel> Areas { get; set; } = new List<EventAreaViewModel>();
    }
}
