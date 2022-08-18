using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

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

        [HttpGet]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public IActionResult GetLayouts()
        {
            var layouts = _layoutService.GetAll()
                .ToList();

            return Ok(layouts);
        }

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

        [HttpGet("{id}")]
        [Authorize(Roles = "Venue manager,Event manager,User")]
        [ProducesResponseType(typeof(LayoutModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLayoutById(int id)
        {
            try
            {
                var layout = await _layoutService.GetByIdAsync(id);

                return Ok(layout);
            }
            catch (ValidationException)
            {
                return NotFound();
            }
        }
    }
}