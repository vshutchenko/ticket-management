using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

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
        [Authorize(Roles = "Venue manager,Event manager,User")]
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
            try
            {
                var venue = await _venueService.GetByIdAsync(id);

                return Ok(venue);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}