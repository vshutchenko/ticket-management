using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("eventAreas")]
    [Produces("application/json")]
    public class EventAreaController : ControllerBase
    {
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;

        public EventAreaController(IEventAreaService eventAreaService, IEventSeatService eventSeatService)
        {
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
            _eventSeatService = eventSeatService ?? throw new ArgumentNullException(nameof(eventSeatService));
        }

        /// <summary>
        /// Get event area by id.
        /// </summary>
        /// <param name="id">Id of the event area.</param>
        /// <returns>Event area.</returns>
        [HttpGet("{id}")]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(EventAreaModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var area = await _eventAreaService.GetByIdAsync(id);

            return Ok(area);
        }

        /// <summary>
        /// Update ticket price for the event area.
        /// </summary>
        /// <param name="id">Id of the event area.</param>
        /// <param name="price">Price of the event area.</param>
        [HttpPut("{id}/price")]
        [AuthorizeRoles(Roles.EventManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAreaPrice(int id, [FromBody] decimal price)
        {
            await _eventAreaService.SetPriceAsync(id, price);

            return NoContent();
        }

        /// <summary>
        /// Get event seats by event area id.
        /// </summary>
        /// <param name="id">Id of the event area.</param>
        /// <returns>List of event seats.</returns>
        [HttpGet("{id}/seats")]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(List<EventSeatModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSeatsByAreaId(int id)
        {
            var seats = _eventSeatService.GetByEventAreaId(id);

            return Ok(seats);
        }
    }
}