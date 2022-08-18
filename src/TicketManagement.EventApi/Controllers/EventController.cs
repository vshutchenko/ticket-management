using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

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

        [HttpGet("{id}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(EventModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var @event = await _eventService.GetByIdAsync(id);

                return Ok(@event);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] EventModel @event)
        {
            try
            {
                var id = await _eventService.CreateAsync(@event);

                return CreatedAtAction(nameof(CreateEvent), new { id = id });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEvent([FromBody] EventModel @event)
        {
            try
            {
                await _eventService.UpdateAsync(@event);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteAsync(id);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}