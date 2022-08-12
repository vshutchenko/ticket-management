using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.EventApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;
        private readonly ILayoutService _layoutService;
        private readonly IVenueService _venueService;
        private readonly IMapper _mapper;

        public EventController(
            IEventService eventService,
            IEventAreaService eventAreaService,
            ILayoutService layoutService,
            IVenueService venueService,
            IMapper mapper)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
            _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IEnumerable<EventModel> Get()
        {
            var events = _eventService.GetAll()
                .Where(e => e.Published)
                .ToList();

            return events;
        }

        [HttpPost]
        public IActionResult Post(EventModel @event)
        {
            var events = _eventService.GetAll()
                .Where(e => e.Published)
                .ToList();

            return events;
        }
    }
}