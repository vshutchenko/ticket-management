using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Venue manager,Event manager")]
    [Route("venues")]
    [Produces("application/json")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VenueModel>), StatusCodes.Status200OK)]
        public IActionResult GetVenues()
        {
            var venues = _venueService.GetAll()
                .ToList();

            return Ok(venues);
        }
    }
}