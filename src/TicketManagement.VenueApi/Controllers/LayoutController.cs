using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Route("layouts")]
    [Produces("application/json")]
    public class LayoutController : ControllerBase
    {
        private readonly ILayoutService _layoutService;

        public LayoutController(ILayoutService layoutService)
        {
            _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
        }

        /// <summary>
        /// Get all layouts.
        /// </summary>
        /// <returns>List of layouts.</returns>
        [HttpGet]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public IActionResult GetLayouts()
        {
            var layouts = _layoutService.GetAll()
                .ToList();

            return Ok(layouts);
        }

        /// <summary>
        /// Get layouts by venue id.
        /// </summary>
        /// <param name="venueId">Id of the venue.</param>
        /// <returns>List of layouts.</returns>
        [HttpGet("venue/{venueId}")]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public IActionResult GetLayoutsByVenueId(int venueId)
        {
            var layouts = _layoutService.GetAll()
                .Where(l => l.VenueId == venueId)
                .ToList();

            return Ok(layouts);
        }

        /// <summary>
        /// Get layout by id.
        /// </summary>
        /// <param name="id">Id of the layout.</param>
        /// <returns>Layout.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(LayoutModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLayoutById(int id)
        {
            var layout = await _layoutService.GetByIdAsync(id);

            return Ok(layout);
        }
    }
}