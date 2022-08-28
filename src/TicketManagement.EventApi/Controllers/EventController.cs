using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("events")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;

        public EventController(IEventService eventService, IEventAreaService eventAreaService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
        }

        /// <summary>
        /// Get events.
        /// </summary>
        /// <returns>List of events.</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EventModel>), StatusCodes.Status200OK)]
        public IActionResult GetEvents(EventFilter filter)
        {
            var events = _eventService
                .GetAll(filter)
                .ToList();

            return Ok(events);
        }

        /// <summary>
        /// Get event by id.
        /// </summary>
        /// <param name="id">Id of the event.</param>
        /// <returns>Event.</returns>
        [HttpGet("{id}")]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(EventModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEventById(int id)
        {
            var @event = await _eventService.GetByIdAsync(id);

            return Ok(@event);
        }

        /// <summary>
        /// Create event.
        /// </summary>
        /// <param name="event">Event to create.</param>
        /// <returns>Id of created event.</returns>
        [HttpPost]
        [AuthorizeRoles(Roles.EventManager)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] EventModel @event)
        {
            var id = await _eventService.CreateAsync(@event);

            return CreatedAtAction(nameof(CreateEvent), id);
        }

        /// <summary>
        /// Update event.
        /// </summary>
        /// <param name="event">Event to update.</param>
        [HttpPut]
        [AuthorizeRoles(Roles.EventManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEvent([FromBody] EventModel @event)
        {
            await _eventService.UpdateAsync(@event);

            return NoContent();
        }

        /// <summary>
        /// Delete event by id.
        /// </summary>
        /// <param name="id">Id of the event to delete.</param>
        [HttpDelete("{id}")]
        [AuthorizeRoles(Roles.EventManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Get event areas by event id.
        /// </summary>
        /// <param name="eventId">Id of the event.</param>
        /// <returns>List of event areas.</returns>
        [HttpGet("{id}/areas")]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(List<EventAreaModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAreasByEventId(int eventId)
        {
            var areas = _eventAreaService.GetByEventId(eventId);

            return Ok(areas);
        }
    }
}