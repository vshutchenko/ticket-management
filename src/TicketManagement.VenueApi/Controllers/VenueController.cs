using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
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
        private readonly ILayoutService _layoutService;

        public VenueController(IVenueService venueService, ILayoutService layoutService)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
        }

        /// <summary>
        /// Get all venues.
        /// </summary>
        /// <returns>List of venues.</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<VenueModel>), StatusCodes.Status200OK)]
        public IActionResult GetVenues()
        {
            var venues = _venueService.GetAll()
                .ToList();

            return Ok(venues);
        }

        /// <summary>
        /// Get layouts by venue id.
        /// </summary>
        /// <param name="id">Id of the venue.</param>
        /// <returns>List of layouts.</returns>
        [HttpGet("{id}/layouts")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLayoutsByVenueId(int id)
        {
            var layouts = await _layoutService.GetByVenueIdAsync(id);

            return Ok(layouts);
        }

        /// <summary>
        /// Get venue by id.
        /// </summary>
        /// <param name="id">Id of the venue.</param>
        /// <returns>Venue.</returns>
        [HttpGet("{id}")]
        [AuthorizeRoles(Roles.VenueManager, Roles.EventManager, Roles.User)]
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
        [AuthorizeRoles(Roles.VenueManager)]
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
        [AuthorizeRoles(Roles.VenueManager)]
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
        [AuthorizeRoles(Roles.VenueManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueService.DeleteAsync(id);

            return NoContent();
        }
    }
}