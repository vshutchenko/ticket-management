using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.WebApplication.Models;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Layout;
using TicketManagement.WebApplication.Models.Venue;

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

        [HttpGet]
        [Authorize(Roles = "Event manager")]
        public PartialViewResult PartialLayoutList(int venueId)
        {
            var layouts = _layoutService.GetAll()
                .Where(l => l.VenueId == venueId)
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            return PartialView(layouts);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            var venues = _venueService.GetAll()
                .Select(v => _mapper.Map<VenueViewModel>(v))
                .ToList();

            var layouts = _layoutService.GetAll()
                .Where(l => l.VenueId == venues.First().Id)
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            var model = new CreateEventViewModel
            {
                Layouts = new SelectList(layouts, "Id", "Description"),
                Venues = new SelectList(venues, "Id", "Description"),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel createModel)
        {
            var @event = _mapper.Map<EventModel>(createModel);

            var id = await _eventService.CreateAsync(@event);

            return RedirectToAction("PublishEvent", new { id = id } );
        }

        [HttpGet]
        [Authorize(Roles = "Event manager")]
        public IActionResult NotPublishedEvents()
        {
            var events = _eventService.GetAll()
                .Where(e => !e.Published)
                .Select(e => _mapper.Map<EventViewModel>(e))
                .ToList();

            return View(events);
        }

        [HttpGet]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> PublishEvent(int id)
        {
            var @event = await _eventService.GetByIdAsync(id);

            var venues = _venueService.GetAll().ToList();
            var layouts = _layoutService.GetAll().ToList();

            var selectedLayout = layouts.First(l => l.Id == @event.LayoutId);
            var selectedVenue = venues.First(v => v.Id == selectedLayout.VenueId);

            var eventVM = @event.Create(layouts, selectedLayout.Id, venues, selectedVenue.Id);

            var areasVM = _eventAreaService.GetByEventId(id)
                .Select(a => _mapper.Map<EventAreaViewModel>(a))
                .ToList();

            var editEventVM = new EditEventViewModel
            {
                Event = eventVM,
                Areas = areasVM,
            };

            return View(editEventVM);
        }

        [HttpPost]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> PublishEvent(Dictionary<string, string> areas)
        {
            foreach (var item in areas)
            {
                int id = int.Parse(item.Key);
                decimal price = decimal.Parse(item.Value);

                await _eventAreaService.SetPriceAsync(id, price);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> EditEvent(int id)
        {
            var @event = await _eventService.GetByIdAsync(id);

            var venues = _venueService.GetAll().ToList();
            var layouts = _layoutService.GetAll().ToList();

            var selectedLayout = layouts.First(l => l.Id == @event.LayoutId);
            var selectedVenue = venues.First(v => v.Id == selectedLayout.VenueId);

            var eventVM = @event.Create(layouts, selectedLayout.Id, venues, selectedVenue.Id);

            var areasVM = _eventAreaService.GetByEventId(id)
                .Select(a => _mapper.Map<EventAreaViewModel>(a))
                .ToList();

            var editEventVM = new EditEventViewModel
            {
                Event = eventVM,
                Areas = areasVM,
            };

            return View(editEventVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> EditEvent(EditEventViewModel m, Dictionary<string, string> areas)
        {
            var @event = _mapper.Map<EventModel>(m.Event!.Id);

            var id = await _eventService.CreateAsync(@event);

            return RedirectToAction("Details", new { id = id });
        }
    }
}
