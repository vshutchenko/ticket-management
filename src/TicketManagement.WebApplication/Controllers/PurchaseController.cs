using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Layout;
using TicketManagement.WebApplication.Models.Purchase;
using TicketManagement.WebApplication.Models.Venue;

namespace TicketManagement.WebApplication.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IPurchaseService _purchaseService;
        private readonly IMapper _mapper;

        public PurchaseController(IEventService eventService, IEventAreaService eventAreaService, IEventSeatService eventSeatService, IPurchaseService purchaseService, IMapper mapper)
        {
            _eventService = eventService;
            _eventAreaService = eventAreaService;
            _eventSeatService = eventSeatService;
            _purchaseService = purchaseService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Roles ="Event manager,User")]
        public async Task<IActionResult> PurchaseSeats(int id)
        {
            var eventVM = _mapper.Map<PurchaseSeatsViewModel>(await _eventService.GetByIdAsync(id));

            foreach (var area in _eventAreaService.GetByEventId(id))
            {
                var areaVM = _mapper.Map<EventAreaViewModel>(area);
                var seatsVM = _eventSeatService.GetByEventAreaId(areaVM.Id).Select(s => _mapper.Map<EventSeatViewModel>(s)).ToList();
                eventVM.Seats.AddRange(seatsVM);
                eventVM.Areas.Add(areaVM);
            }

            return View(eventVM);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseSeats(CreatePurchaseViewModel model)
        {
            var purchase = _mapper.Map<PurchaseModel>(model);

            var seats = new List<EventSeatModel>();

            foreach (var id in model.SeatIds)
            {
                var s = await _eventSeatService.GetByIdAsync(id);
                seats.Add(_mapper.Map<EventSeatModel>(s));
            }

            await _purchaseService.PurchaseSeatAsync(purchase);

            return RedirectToAction("PurchaseHistory", "Purchase");
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PurchaseHistory(
            [FromServices] IVenueService venueService,
            [FromServices] ILayoutService layoutService)
        {
            var userId = User.FindFirstValue("id");

            IEnumerable<PurchaseModel>? purchases = _purchaseService.GetByUserId(userId);

            var models = new List<PurchaseViewModel>();

            foreach (var purchase in purchases)
            {
                var purchasedSeats = new List<PurchasedSeatViewModel>();

                foreach (var id in purchase.SeatIds)
                {
                    var seat = _mapper.Map<EventSeatViewModel>(await _eventSeatService.GetByIdAsync(id));
                    var area = _mapper.Map<EventAreaViewModel>(await _eventAreaService.GetByIdAsync(seat.EventAreaId));
                    var @event = _mapper.Map<EventViewModel>(await _eventService.GetByIdAsync(area.EventId));
                    var layout = _mapper.Map<LayoutViewModel>(await layoutService.GetByIdAsync(@event.LayoutId));
                    var venue = _mapper.Map<VenueViewModel>(await venueService.GetByIdAsync(layout.VenueId));

                    var purchasedSeatVm = new PurchasedSeatViewModel { Area = area, Seat = seat,  Event = @event, Layout = layout, Venue = venue };

                    purchasedSeats.Add(purchasedSeatVm);
                }

                var purchaseVm = new PurchaseViewModel { PurchasedSeats = purchasedSeats, Price = purchase.Price };

                models.Add(purchaseVm);
            }

            return View(models);
        }
    }
}
