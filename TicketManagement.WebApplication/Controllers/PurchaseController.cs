using System.Security.Claims;
using AutoMapper;
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
        private readonly IPurchaseService _purchaseService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IMapper _mapper;

        public PurchaseController(IPurchaseService purchaseService, IEventSeatService eventSeatService, IMapper mapper)
        {
            _purchaseService = purchaseService;
            _eventSeatService = eventSeatService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseSeats(CreatePurchaseViewModel model)
        {
            var purchase = _mapper.Map<PurchaseModel>(model);

            List<EventSeatModel> seats = new List<EventSeatModel>();

            foreach (var id in model.SeatIds)
            {
                var s = await _eventSeatService.GetByIdAsync(id);
                seats.Add(_mapper.Map<EventSeatModel>(s));
            }

            await _purchaseService.PurchaseSeatAsync(purchase);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PurchaseHistory(
            [FromServices] IEventService eventService,
            [FromServices] IVenueService venueService,
            [FromServices] ILayoutService layoutService,
            [FromServices] IEventAreaService eventAreaService)
        {
            var userId = User.FindFirstValue("id");

            var purchases = _purchaseService.GetByUserId(userId);

            List<PurchaseViewModel> models = new List<PurchaseViewModel>();

            foreach (var purchase in purchases)
            {
                var purchasedSeats = new List<PurchasedSeatViewModel>();

                foreach (var id in purchase.SeatIds)
                {
                    var seat = _mapper.Map<EventSeatViewModel>(await _eventSeatService.GetByIdAsync(id));
                    var area = _mapper.Map<EventAreaViewModel>(await eventAreaService.GetByIdAsync(seat.EventAreaId));
                    var @event = _mapper.Map<EventViewModel>(await eventService.GetByIdAsync(area.EventId));
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
