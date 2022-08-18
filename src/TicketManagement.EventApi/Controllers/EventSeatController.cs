using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

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

        [HttpGet("areas/{areaId}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(List<EventSeatModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetSeatsByAreaId(int areaId)
        {
            try
            {
                var seats = _eventSeatService.GetByEventAreaId(areaId);

                return Ok(seats);
            }
            catch (ValidationException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(EventSeatModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var seat = await _eventSeatService.GetByIdAsync(id);

                return Ok(seat);
            }
            catch (ValidationException)
            {
                return NotFound();
            }
        }

        [HttpPut("{seatId}/state")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAreaPrice(int seatId, [FromBody] int state)
        {
            try
            {
                await _eventSeatService.SetSeatStateAsync(seatId, (EventSeatStateModel)state);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}