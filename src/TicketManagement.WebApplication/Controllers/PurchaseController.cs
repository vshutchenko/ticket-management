using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Clients.EventApi;
using TicketManagement.WebApplication.Clients.EventApi.Models;
using TicketManagement.WebApplication.Clients.PurchaseApi;
using TicketManagement.WebApplication.Clients.PurchaseApi.Models;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Purchase;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IEventClient _eventClient;
        private readonly IPurchaseClient _purchaseClient;
        private readonly IEventAreaClient _eventAreaClient;
        private readonly IEventSeatClient _eventSeatClient;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public PurchaseController(IEventClient eventClient,
            IPurchaseClient purchaseClient,
            IEventAreaClient eventAreaClient,
            IEventSeatClient eventSeatClient,
            ITokenService tokenService,
            IMapper mapper)
        {
            _purchaseClient = purchaseClient ?? throw new ArgumentNullException(nameof(purchaseClient));
            _eventAreaClient = eventAreaClient ?? throw new ArgumentNullException(nameof(eventAreaClient));
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _eventSeatClient = eventSeatClient ?? throw new ArgumentNullException(nameof(eventSeatClient));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        public async Task<IActionResult> PurchaseSeats(int id)
        {
            var @event = await _eventClient.GetByIdAsync(id, _tokenService.GetToken());

            var eventVM = _mapper.Map<PurchaseSeatsViewModel>(@event);

            var areas = await _eventClient.GetAreasByEventIdAsync(id, _tokenService.GetToken());

            foreach (var area in areas)
            {
                var areaVM = _mapper.Map<EventAreaViewModel>(area);

                var seats = await _eventAreaClient.GetSeatsByAreaIdAsync(areaVM.Id, _tokenService.GetToken());

                var seatsVM = seats.Select(s => _mapper.Map<EventSeatViewModel>(s)).ToList();

                eventVM.Seats.AddRange(seatsVM);

                eventVM.Areas.Add(areaVM);
            }

            return View(eventVM);
        }

        [HttpPost]
        [AuthorizeRoles(Roles.User)]
        public async Task<IActionResult> PurchaseSeats(CreatePurchaseViewModel model)
        {
            var purchase = _mapper.Map<PurchaseModel>(model);

            var seats = new List<EventSeatModel>();

            foreach (var id in model.SeatIds)
            {
                var s = await _eventSeatClient.GetByIdAsync(id, _tokenService.GetToken());
                seats.Add(_mapper.Map<EventSeatModel>(s));
            }

            await _purchaseClient.PurchaseAsync(purchase, _tokenService.GetToken());

            return RedirectToAction("PurchaseHistory", "Purchase");
        }

        [HttpGet]
        [AuthorizeRoles(Roles.User)]
        public async Task<IActionResult> PurchaseHistory(
            [FromServices] ILayoutClient layoutClient,
            [FromServices] IVenueClient venueClient)
        {
            var userId = User.FindFirstValue("id");

            var purchases = await _purchaseClient.GetByUserIdAsync(userId, _tokenService.GetToken());

            var models = new List<PurchaseViewModel>();

            foreach (var purchase in purchases)
            {
                var purchasedSeats = new List<PurchasedSeatViewModel>();

                foreach (var id in purchase.SeatIds)
                {
                    var seat = _mapper.Map<EventSeatViewModel>(await _eventSeatClient.GetByIdAsync(id, _tokenService.GetToken()));
                    var area = _mapper.Map<EventAreaViewModel>(await _eventAreaClient.GetByIdAsync(seat.EventAreaId, _tokenService.GetToken()));
                    var @event = _mapper.Map<EventViewModel>(await _eventClient.GetByIdAsync(area.EventId, _tokenService.GetToken()));
                    var layout = _mapper.Map<LayoutViewModel>(await layoutClient.GetByIdAsync(@event.LayoutId, _tokenService.GetToken()));
                    var venue = _mapper.Map<VenueViewModel>(await venueClient.GetByIdAsync(layout.VenueId, _tokenService.GetToken()));

                    var purchasedSeatVm = new PurchasedSeatViewModel { Area = area, Seat = seat, Event = @event, Layout = layout, Venue = venue };

                    purchasedSeats.Add(purchasedSeatVm);
                }

                var purchaseVM = new PurchaseViewModel { PurchasedSeats = purchasedSeats, Price = purchase.Price };

                models.Add(purchaseVM);
            }

            return View(models);
        }
    }
}