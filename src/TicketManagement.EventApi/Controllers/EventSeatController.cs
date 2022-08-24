using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("event-seats")]
    [Produces("application/json")]
    public class EventSeatController : ControllerBase
    {
        private readonly IEventSeatService _eventSeatService;

        public EventSeatController(IEventSeatService eventSeatService)
        {
            _eventSeatService = eventSeatService ?? throw new ArgumentNullException(nameof(eventSeatService));
        }

        /// <summary>
        /// Get event seats by event area id.
        /// </summary>
        /// <param name="areaId">Id of the event area.</param>
        /// <returns>List of event seats.</returns>
        [HttpGet("areas/{areaId}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(List<EventSeatModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSeatsByAreaId(int areaId)
        {
            var seats = _eventSeatService.GetByEventAreaId(areaId);

            return Ok(seats);
        }

        /// <summary>
        /// Get event seat by id.
        /// </summary>
        /// <param name="id">Id of the event seat.</param>
        /// <returns>Event seat.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Event manager,User")]
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
        /// <param name="seatId">Id of the event seat.</param>
        /// <param name="state">State of the event seat.</param>
        [HttpPut("{seatId}/state")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAreaPrice(int seatId, [FromBody] EventSeatStateModel state)
        {
            await _eventSeatService.SetSeatStateAsync(seatId, state);

            return NoContent();
        }
    }
}