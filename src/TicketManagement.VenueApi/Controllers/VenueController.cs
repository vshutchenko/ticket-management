using Microsoft.AspNetCore.Mvc;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        }

        [HttpGet]
        public IEnumerable<VenueModel> Get()
        {
            var venues = _venueService.GetAll()
                .ToList();

            return venues;
        }
    }
}