using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Clients.EventApi;
using TicketManagement.Core.Clients.EventApi.Models;
using TicketManagement.Core.Models;

namespace TicketManagement.WebApplication.Controllers
{
    [ApiController]
    [Route("api/events")]
    [AuthorizeRoles(Roles.EventManager)]
    public class EventController : ControllerBase
    {
        private readonly IEventClient _eventClient;
        private readonly IEventAreaClient _eventAreaClient;

        public EventController(
            IEventClient eventClient,
            IEventAreaClient eventAreaClient)
        {
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _eventAreaClient = eventAreaClient ?? throw new ArgumentNullException(nameof(eventAreaClient));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublishedEvents()
        {
            var events = await _eventClient.GetEventsAsync(EventFilter.Published, string.Empty);

            return Ok(events);
        }
    }
}
