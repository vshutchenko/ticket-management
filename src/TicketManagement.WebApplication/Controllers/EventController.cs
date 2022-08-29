using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Clients.EventApi;
using TicketManagement.WebApplication.Clients.EventApi.Models;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Extensions;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.EventManager)]
    public class EventController : BaseController
    {
        private readonly IEventClient _eventClient;
        private readonly IEventAreaClient _eventAreaClient;
        private readonly IMapper _mapper;

        public EventController(
            IEventClient eventClient,
            IEventAreaClient eventAreaClient,
            ITokenService tokenService,
            IMapper mapper)
            : base(tokenService)
        {
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _eventAreaClient = eventAreaClient ?? throw new ArgumentNullException(nameof(eventAreaClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var events = await _eventClient.GetEventsAsync(EventFilter.Published, TokenService.GetToken());

            var eventsVM = events
                .Select(e => _mapper.Map<EventViewModel>(e))
                .ToList();

            return View(eventsVM);
        }

        [HttpGet]
        public async Task<IActionResult> NotPublishedEvents()
        {
            var events = await _eventClient.GetEventsAsync(EventFilter.NotPublished, TokenService.GetToken());

            var eventsVM = events
                .Select(e => _mapper.Map<EventViewModel>(e))
                .ToList();

            return View(eventsVM);
        }

        [HttpGet]
        public async Task<PartialViewResult> PartialLayoutList(int venueId, [FromServices] ILayoutClient layoutClient)
        {
            var layouts = await layoutClient.GetByVenueIdAsync(venueId, TokenService.GetToken());

            var layoutsVM = layouts
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            return PartialView(layoutsVM);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEvent(
            [FromServices] ILayoutClient layoutClient,
            [FromServices] IVenueClient venueClient)
        {
            var venues = await venueClient.GetAllAsync(TokenService.GetToken());

            var venuesVM = venues
                .Select(v => _mapper.Map<VenueViewModel>(v))
                .ToList();

            var layouts = await layoutClient.GetByVenueIdAsync(venues.First().Id, TokenService.GetToken());

            var layoutsVM = layouts
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            var model = new CreateEventViewModel
            {
                Layouts = new SelectList(layoutsVM, "Id", "Description"),
                Venues = new SelectList(venuesVM, "Id", "Description"),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel createModel)
        {
            var @event = _mapper.Map<EventModel>(createModel);

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            var id = await _eventClient.CreateAsync(@event, TokenService.GetToken());

            return RedirectToAction(nameof(EditEvent), new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id,
            [FromServices] ILayoutClient layoutClient,
            [FromServices] IVenueClient venueClient)
        {
            var @event = await _eventClient.GetByIdAsync(id, TokenService.GetToken());

            var venues = await venueClient.GetAllAsync(TokenService.GetToken());

            var layouts = await layoutClient.GetAllAsync(TokenService.GetToken());

            var venuesVM = venues
                .Select(v => _mapper.Map<VenueViewModel>(v))
                .ToList();

            var layoutsVM = layouts
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            var selectedLayout = layoutsVM.First(l => l.Id == @event.LayoutId);
            var selectedVenue = venuesVM.First(v => v.Id == selectedLayout.VenueId);

            var eventVM = @event.Create(layouts.Where(l => l.VenueId == selectedVenue.Id).ToList(), selectedLayout.Id, venues, selectedVenue.Id);

            var areas = await _eventClient.GetAreasByEventIdAsync(id, TokenService.GetToken());

            var areasVM = areas
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
        public async Task<IActionResult> EditNotPublishedEvent(EditEventViewModel model)
        {
            var @event = _mapper.Map<EventModel>(model.Event);

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            await _eventClient.UpdateAsync(@event, TokenService.GetToken());

            foreach (var item in model.Areas)
            {
                await _eventAreaClient.UpdatePriceAsync(item.Id, item.Price, TokenService.GetToken());
            }

            return RedirectToAction(nameof(PurchaseController.PurchaseSeats), nameof(PurchaseController), new { id = @event.Id });
        }

        [HttpPost]
        public async Task<IActionResult> EditPublishedEvent(EditEventViewModel model)
        {
            var @event = _mapper.Map<EventModel>(model.Event);

            @event.StartDate = User.GetUtcTime(@event.StartDate);
            @event.EndDate = User.GetUtcTime(@event.EndDate);

            await _eventClient.UpdateAsync(@event, TokenService.GetToken());

            return RedirectToAction(nameof(EditEvent), new { id = @event.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id, [FromServices] IWebHostEnvironment enviroment)
        {
            var @event = await _eventClient.GetByIdAsync(id, TokenService.GetToken());

            var imagePath = Path.Combine(enviroment.WebRootPath, $"{@event!.ImageUrl}");

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            await _eventClient.DeleteAsync(id, TokenService.GetToken());

            return RedirectToAction(nameof(Index));
        }
    }
}
