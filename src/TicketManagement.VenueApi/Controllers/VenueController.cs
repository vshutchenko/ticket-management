using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Route("venues")]
    [Produces("application/json")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        }

        /// <summary>
        /// Get all venues.
        /// </summary>
        /// <returns>List of venues.</returns>
        [HttpGet]
        [Authorize(Roles = "Venue manager,Event manager")]
        [ProducesResponseType(typeof(List<VenueModel>), StatusCodes.Status200OK)]
        public IActionResult GetVenues()
        {
            var venues = _venueService.GetAll()
                .ToList();

            return Ok(venues);
        }

        /// <summary>
        /// Get venue by id.
        /// </summary>
        /// <param name="id">Id of the venue.</param>
        /// <returns>Venue.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(VenueModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);

            return Ok(venue);
        }

        /// <summary>
        /// Create venue.
        /// </summary>
        /// <param name="venue">Venue to create.</param>
        /// <returns>Id of created venue.</returns>
        [HttpPost]
        [Authorize(Roles = "Venue manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateVenue([FromBody] VenueModel venue)
        {
            var id = await _venueService.CreateAsync(venue);

            return CreatedAtAction(nameof(CreateVenue), id);
        }

        /// <summary>
        /// Update venue.
        /// </summary>
        /// <param name="venue">Venue to update.</param>
        [HttpPut]
        [Authorize(Roles = "Venue manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVenue([FromBody] VenueModel venue)
        {
            await _venueService.UpdateAsync(venue);

            return NoContent();
        }

        /// <summary>
        /// Delete venue by id.
        /// </summary>
        /// <param name="id">Id of the venue to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Venue manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueService.DeleteAsync(id);

            return NoContent();
        }
    }
}