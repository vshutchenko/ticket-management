using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.WebApplication.Models;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;

namespace TicketManagement.WebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;
        private readonly ILayoutService _layoutService;
        private readonly IVenueService _venueService;
        private readonly IMapper _mapper;
        private readonly int _pageSize = 10;

        public EventController(
            IEventService eventService,
            IEventAreaService eventAreaService,
            IEventSeatService eventSeatService,
            ILayoutService layoutService,
            IVenueService venueService,
            IMapper mapper)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
            _eventSeatService = eventSeatService ?? throw new ArgumentNullException(nameof(eventSeatService));
            _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var eventsCount = _eventService.Count();

            var eventsVM = _eventService
                .Get(_pageSize, _pageSize * (page - 1))
                .Select(e => _mapper.Map<EventViewModel>(e));

            var pagingInfo = new PagingInfo(eventsCount, _pageSize, page);

            var listVM = new EventListViewModel(eventsVM, pagingInfo);

            return View(listVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var eventVM = _mapper.Map<EventDetailsViewModel>(await _eventService.GetByIdAsync(id));

            foreach (var area in _eventAreaService.GetByEventId(id))
            {
                var areaVM = _mapper.Map<EventAreaViewModel>(area);
                eventVM.Seats = _eventSeatService.GetByEventAreaId(areaVM.Id).Select(s => _mapper.Map<EventSeatViewModel>(s)).ToList();
                eventVM.Areas.Add(areaVM);
            }

            return View(eventVM);
        }
    }
}
