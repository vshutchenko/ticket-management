using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Venue manager, Event manager")]
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
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public IActionResult GetLayouts()
        {
            var layouts = _layoutService.GetAll()
                .ToList();

            return Ok(layouts);
        }

        [HttpGet("{venueId}")]
        [Authorize(Roles = "Event manager")]
        [ProducesResponseType(typeof(List<LayoutModel>), StatusCodes.Status200OK)]
        public IActionResult GetLayoutsByVenueId(int venueId)
        {
            var layouts = _layoutService.GetAll()
                .Where(l => l.VenueId == venueId)
                .ToList();

            return Ok(layouts);
        }
    }
}