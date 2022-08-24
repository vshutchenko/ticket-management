using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public EventController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Get published events.
        /// </summary>
        /// <returns>List of published events.</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EventModel>), StatusCodes.Status200OK)]
        public IActionResult GetPublishedEvents()
        {
            var events = _eventService.GetAll()
                .Where(e => e.Published)
                .ToList();

            return Ok(events);
        }

        /// <summary>
        /// Get not published events.
        /// </summary>
        /// <returns>List of not published events.</returns>
        [HttpGet("not-published")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(typeof(List<EventModel>), StatusCodes.Status200OK)]
        public IActionResult GetNotPublishedEvents()
        {
            var events = _eventService.GetAll()
                .Where(e => !e.Published)
                .ToList();

            return Ok(events);
        }

        /// <summary>
        /// Get event by id.
        /// </summary>
        /// <param name="id">Id of the event.</param>
        /// <returns>Event.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Event manager,User")]
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
        [Authorize(Roles = "Event manager")]
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
        [Authorize(Roles = "Event manager")]
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
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteAsync(id);

            return NoContent();
        }
    }
}