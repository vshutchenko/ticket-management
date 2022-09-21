using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [AuthorizeRoles(Roles.VenueManager)]
    [Route("areas")]
    [Produces("application/json")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        public AreaController(IAreaService areaService, ISeatService seatService)
        {
            _areaService = areaService ?? throw new ArgumentNullException(nameof(areaService));
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
        }

        /// <summary>
        /// Get all areas.
        /// </summary>
        /// <returns>List of areas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AreaModel>), StatusCodes.Status200OK)]
        public IActionResult GetAreas()
        {
            var areas = _areaService.GetAll()
                .ToList();

            return Ok(areas);
        }

        /// <summary>
        /// Get seats by area id.
        /// </summary>
        /// <param name="id">Id of the area.</param>
        /// <returns>List of seats.</returns>
        [HttpGet("{id}/seats")]
        [ProducesResponseType(typeof(List<SeatModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeatsByAreaId(int id)
        {
            var seats = await _seatService.GetByAreaIdAsync(id);

            return Ok(seats);
        }

        /// <summary>
        /// Get area by id.
        /// </summary>
        /// <param name="id">Id of the area.</param>
        /// <returns>Area.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AreaModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var area = await _areaService.GetByIdAsync(id);

            return Ok(area);
        }

        /// <summary>
        /// Create area.
        /// </summary>
        /// <param name="area">Area to create.</param>
        /// <returns>Id of created area.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateArea([FromBody] AreaModel area)
        {
            var id = await _areaService.CreateAsync(area);

            return CreatedAtAction(nameof(CreateArea), id);
        }

        /// <summary>
        /// Update area.
        /// </summary>
        /// <param name="area">Area to update.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateArea([FromBody] AreaModel area)
        {
            await _areaService.UpdateAsync(area);

            return NoContent();
        }

        /// <summary>
        /// Delete area by id.
        /// </summary>
        /// <param name="id">Id of the area to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteArea(int id)
        {
            await _areaService.DeleteAsync(id);

            return NoContent();
        }
    }
}