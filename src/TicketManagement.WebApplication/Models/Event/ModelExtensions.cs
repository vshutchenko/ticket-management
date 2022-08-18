using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.WebApplication.Clients.EventApi.Models;
using TicketManagement.WebApplication.Clients.VenueApi.Models;

namespace TicketManagement.WebApplication.Models.Event
{
    public static class ModelExtensions
    {
        public static CreateEventViewModel Create(this EventModel @event, List<LayoutModel> layouts, int layoutId, List<VenueModel> venues, int venueId)
        {
            return new CreateEventViewModel
            {
                Id = @event.Id,
                Name = @event.Name!,
                Description = @event.Description!,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                ImageUrl = @event.ImageUrl,
                Layout = layoutId.ToString(),
                Venue = venueId.ToString(),
                Layouts = new SelectList(layouts, "Id", "Description"),
                Venues = new SelectList(venues, "Id", "Description"),
                Published = @event.Published,
            };
        }
    }
}
