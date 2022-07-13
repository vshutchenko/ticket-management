using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.WebApplication.Extensions;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.Layout;
using TicketManagement.WebApplication.Models.Venue;

namespace TicketManagement.WebApplication.Controllers
{
    public class EventController : Controller
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
        public IActionResult Index()
        {
            var eventsVM = _eventService.GetAll()
                .Where(e => e.Published)
                .Select(e => _mapper.Map<EventViewModel>(e))
                .ToList();

            return View(eventsVM);
        }

        [HttpGet]
        public IActionResult NotPublishedEvents()
        {
            var eventsVM = _eventService.GetAll()
                .Where(e => !e.Published)
                .Select(e => _mapper.Map<EventViewModel>(e))
                .ToList();

            return View(eventsVM);
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
        [Authorize(Roles = "Event manager")]
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

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            var id = await _eventService.CreateAsync(@event);

            return RedirectToAction("EditEvent", new { id = id } );
        }

        [HttpGet]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> EditEvent(int id)
        {
            var @event = await _eventService.GetByIdAsync(id);

            List<VenueModel>? venues = _venueService.GetAll().ToList();
            List<LayoutModel>? layouts = _layoutService.GetAll().ToList();

            var selectedLayout = layouts.First(l => l.Id == @event.LayoutId);
            var selectedVenue = venues.First(v => v.Id == selectedLayout.VenueId);

            var eventVM = @event.Create(layouts.Where(l => l.VenueId == selectedVenue.Id).ToList(), selectedLayout.Id, venues, selectedVenue.Id);

            var areasVM = _eventAreaService.GetByEventId(id)
                .Select(a => _mapper.Map<EventAreaViewModel>(a))
                .ToList();

            var editEventVM = new EditEventViewModel
            {
                Event = eventVM,
                Areas = areasVM,
            };

            return eventVM.Published
                ? View("EditPublishedEvent", editEventVM)
                : View("EditNotPublishedEvent", editEventVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> EditNotPublishedEvent(EditEventViewModel model)
        {
            foreach (var item in model.Areas)
            {
                await _eventAreaService.SetPriceAsync(item.Id, item.Price);
            }

            var @event = _mapper.Map<EventModel>(model.Event);

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            await _eventService.UpdateAsync(@event);

            return RedirectToAction("PurchaseSeats", "Purchase",  new { id = @event.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> EditPublishedEvent(EditEventViewModel model)
        {
            var @event = _mapper.Map<EventModel>(model.Event);

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            await _eventService.UpdateAsync(@event);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Event manager")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteAsync(id);

            return RedirectToAction("Index");
        }
    }
}
