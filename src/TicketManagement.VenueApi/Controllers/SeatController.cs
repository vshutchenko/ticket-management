using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Venue manager")]
    [Route("seats")]
    [Produces("application/json")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
        }

        /// <summary>
        /// Get all seats.
        /// </summary>
        /// <returns>List of seats.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SeatModel>), StatusCodes.Status200OK)]
        public IActionResult GetSeats()
        {
            var seats = _seatService.GetAll()
                .ToList();

            return Ok(seats);
        }

        /// <summary>
        /// Get seats by area id.
        /// </summary>
        /// <param name="areaId">Id of the area.</param>
        /// <returns>List of seats.</returns>
        [HttpGet("areas/{areaId}")]
        [ProducesResponseType(typeof(List<SeatModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSatsByAreaId(int areaId)
        {
            var seats = await _seatService.GetByAreaIdAsync(areaId);

            return Ok(seats);
        }

        /// <summary>
        /// Get seat by id.
        /// </summary>
        /// <param name="id">Id of the seat.</param>
        /// <returns>Seat.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeatModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            return Ok(seat);
        }

        /// <summary>
        /// Create seat.
        /// </summary>
        /// <param name="seat">Seat to create.</param>
        /// <returns>Id of created seat.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSeat([FromBody] SeatModel seat)
        {
            var id = await _seatService.CreateAsync(seat);

            return CreatedAtAction(nameof(CreateSeat), id);
        }

        /// <summary>
        /// Update seat.
        /// </summary>
        /// <param name="seat">Seat to update.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSeat([FromBody] SeatModel seat)
        {
            await _seatService.UpdateAsync(seat);

            return NoContent();
        }

        /// <summary>
        /// Delete seat by id.
        /// </summary>
        /// <param name="id">Id of the seat to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            await _seatService.DeleteAsync(id);

            return NoContent();
        }
    }
}