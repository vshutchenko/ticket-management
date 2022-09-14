using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("eventSeats")]
    [Produces("application/json")]
    public class EventSeatController : ControllerBase
    {
        private readonly IEventSeatService _eventSeatService;

        public EventSeatController(IEventSeatService eventSeatService)
        {
            _eventSeatService = eventSeatService ?? throw new ArgumentNullException(nameof(eventSeatService));
        }

        /// <summary>
        /// Get event seat by id.
        /// </summary>
        /// <param name="id">Id of the event seat.</param>
        /// <returns>Event seat.</returns>
        [HttpGet("{id}")]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(EventSeatModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await _eventSeatService.GetByIdAsync(id);

            return Ok(seat);
        }

        /// <summary>
        /// Update state for the event seat.
        /// </summary>
        /// <param name="id">Id of the event seat.</param>
        /// <param name="state">State of the event seat.</param>
        [HttpPut("{id}/state")]
        [AuthorizeRoles(Roles.EventManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSeatState(int id, [FromBody] EventSeatState state)
        {
            await _eventSeatService.SetSeatStateAsync(id, state);

            return NoContent();
        }
    }
}