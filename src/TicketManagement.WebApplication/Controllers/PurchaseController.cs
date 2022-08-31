using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Clients.EventApi;
using TicketManagement.Core.Clients.EventApi.Models;
using TicketManagement.Core.Clients.PurchaseApi;
using TicketManagement.Core.Clients.PurchaseApi.Models;
using TicketManagement.Core.Clients.VenueApi;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Purchase;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    public class PurchaseController : BaseController
    {
        private readonly IEventClient _eventClient;
        private readonly IPurchaseClient _purchaseClient;
        private readonly IEventAreaClient _eventAreaClient;
        private readonly IEventSeatClient _eventSeatClient;
        private readonly IMapper _mapper;

        public PurchaseController(
            IEventClient eventClient,
            IPurchaseClient purchaseClient,
            IEventAreaClient eventAreaClient,
            IEventSeatClient eventSeatClient,
            ITokenService tokenService,
            IMapper mapper)
            : base(tokenService)
        {
            _purchaseClient = purchaseClient ?? throw new ArgumentNullException(nameof(purchaseClient));
            _eventAreaClient = eventAreaClient ?? throw new ArgumentNullException(nameof(eventAreaClient));
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _eventSeatClient = eventSeatClient ?? throw new ArgumentNullException(nameof(eventSeatClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AuthorizeRoles(Roles.EventManager, Roles.User)]
        public async Task<IActionResult> PurchaseSeats(int id)
        {
            var @event = await _eventClient.GetByIdAsync(id, TokenService.GetToken());

            var eventVM = _mapper.Map<PurchaseSeatsViewModel>(@event);

            var areas = await _eventClient.GetAreasByEventIdAsync(id, TokenService.GetToken());

            foreach (var area in areas)
            {
                var areaVM = _mapper.Map<EventAreaViewModel>(area);

                var seats = await _eventAreaClient.GetSeatsByAreaIdAsync(areaVM.Id, TokenService.GetToken());

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
                var s = await _eventSeatClient.GetByIdAsync(id, TokenService.GetToken());
                seats.Add(_mapper.Map<EventSeatModel>(s));
            }

            await _purchaseClient.PurchaseAsync(purchase, TokenService.GetToken());

            return RedirectToAction(nameof(PurchaseHistory));
        }

        [HttpGet]
        [AuthorizeRoles(Roles.User)]
        public async Task<IActionResult> PurchaseHistory(
            [FromServices] ILayoutClient layoutClient,
            [FromServices] IVenueClient venueClient)
        {
            var userId = User.FindFirstValue("id");

            var purchases = await _purchaseClient.GetByUserIdAsync(userId, TokenService.GetToken());

            var models = new List<PurchaseViewModel>();

            foreach (var purchase in purchases)
            {
                var purchasedSeats = new List<PurchasedSeatViewModel>();

                foreach (var id in purchase.SeatIds)
                {
                    var seat = _mapper.Map<EventSeatViewModel>(await _eventSeatClient.GetByIdAsync(id, TokenService.GetToken()));
                    var area = _mapper.Map<EventAreaViewModel>(await _eventAreaClient.GetByIdAsync(seat.EventAreaId, TokenService.GetToken()));
                    var @event = _mapper.Map<EventViewModel>(await _eventClient.GetByIdAsync(area.EventId, TokenService.GetToken()));
                    var layout = _mapper.Map<LayoutViewModel>(await layoutClient.GetByIdAsync(@event.LayoutId, TokenService.GetToken()));
                    var venue = _mapper.Map<VenueViewModel>(await venueClient.GetByIdAsync(layout.VenueId, TokenService.GetToken()));

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