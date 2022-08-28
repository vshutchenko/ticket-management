using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
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
        [AuthorizeRoles(Roles.VenueManager)]
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
        [HttpGet("venues/{venueId}")]
        [AuthorizeRoles(Roles.EventManager, Roles.VenueManager)]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLayoutsByVenueId(int venueId)
        {
            var layouts = await _layoutService.GetByVenueIdAsync(venueId);

            return Ok(layouts);
        }

        /// <summary>
        /// Get layout by id.
        /// </summary>
        /// <param name="id">Id of the layout.</param>
        /// <returns>Layout.</returns>
        [HttpGet("{id}")]
        [AuthorizeRoles(Roles.VenueManager, Roles.EventManager, Roles.User)]
        [ProducesResponseType(typeof(LayoutModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLayoutById(int id)
        {
            var layout = await _layoutService.GetByIdAsync(id);

            return Ok(layout);
        }

        /// <summary>
        /// Create layout.
        /// </summary>
        /// <param name="layout">Layout to create.</param>
        /// <returns>Id of created layout.</returns>
        [HttpPost]
        [AuthorizeRoles(Roles.VenueManager)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLayout([FromBody] LayoutModel layout)
        {
            var id = await _layoutService.CreateAsync(layout);

            return CreatedAtAction(nameof(CreateLayout), id);
        }

        /// <summary>
        /// Update layout.
        /// </summary>
        /// <param name="layout">Layout to update.</param>
        [HttpPut]
        [AuthorizeRoles(Roles.VenueManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLayout([FromBody] LayoutModel layout)
        {
            await _layoutService.UpdateAsync(layout);

            return NoContent();
        }

        /// <summary>
        /// Delete layout by id.
        /// </summary>
        /// <param name="id">Id of the layout to delete.</param>
        [HttpDelete("{id}")]
        [AuthorizeRoles(Roles.VenueManager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLayout(int id)
        {
            await _layoutService.DeleteAsync(id);

            return NoContent();
        }
    }
}