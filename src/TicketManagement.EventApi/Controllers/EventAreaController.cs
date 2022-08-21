using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("event-areas")]
    [Produces("application/json")]
    public class EventAreaController : ControllerBase
    {
        private readonly IEventAreaService _eventAreaService;

        public EventAreaController(IEventAreaService eventAreaService)
        {
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
        }

        /// <summary>
        /// Get event areas by event id.
        /// </summary>
        /// <param name="eventId">Id of the event.</param>
        /// <returns>List of event areas.</returns>
        [HttpGet("events/{eventId}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(List<EventAreaModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAreasByEventId(int eventId)
        {
            try
            {
                var areas = _eventAreaService.GetByEventId(eventId);

                return Ok(areas);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get event area by id.
        /// </summary>
        /// <param name="id">Id of the event area.</param>
        /// <returns>Event area.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(typeof(EventAreaModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAreaById(int id)
        {
            try
            {
                var area = await _eventAreaService.GetByIdAsync(id);

                return Ok(area);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update ticket price for the event area.
        /// </summary>
        /// <param name="areaId">Id of the event area.</param>
        /// <param name="price">Price of the event area.</param>
        [HttpPut("{areaId}/price")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAreaPrice(int areaId, [FromBody] decimal price)
        {
            try
            {
                await _eventAreaService.SetPriceAsync(areaId, price);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}