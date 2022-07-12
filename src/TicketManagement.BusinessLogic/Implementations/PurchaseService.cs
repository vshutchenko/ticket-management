using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class PurchaseService : IPurchaseService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<PurchasedSeat> _purchasedSeatRepository;
        private readonly IEventSeatService _eventSeatService;
        private readonly IEventAreaService _eventAreaService;

        public PurchaseService(
            UserManager<User> userManager,
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchasedSeat> purchasedSeatRepository,
            IEventSeatService eventSeatService,
            IEventAreaService eventAreaService)
        {
            _userManager = userManager;
            _purchaseRepository = purchaseRepository;
            _purchasedSeatRepository = purchasedSeatRepository;
            _eventSeatService = eventSeatService;
            _eventAreaService = eventAreaService;
        }

        public async Task PurchaseSeatAsync(PurchaseModel model)
        {
            await CheckSeatsAsync(model.SeatIds);

            Purchase purchase = new Purchase
            {
                EventId = model.EventId,
                UserId = model.UserId,
                Price = await CalculatePriceAsync(model.SeatIds),
            };

            await MakePayment(purchase.UserId, purchase.Price);

            int purchaseId = await _purchaseRepository.CreateAsync(purchase);

            foreach (int id in model.SeatIds)
            {
                await _eventSeatService.SetSeatStateAsync(id, EventSeatStateModel.Ordered);

                PurchasedSeat purchasedSeat = new PurchasedSeat { EventSeatId = id, PurchaseId = purchaseId };

                await _purchasedSeatRepository.CreateAsync(purchasedSeat);
            }
        }

        public IEnumerable<PurchaseModel> GetByUserId(string userId)
        {
            List<Purchase> purchases = _purchaseRepository.GetAll().Where(p => p.UserId == userId).ToList();

            List<PurchaseModel> models = new List<PurchaseModel>();

            foreach (Purchase p in purchases)
            {
                PurchaseModel purchaseModel = new PurchaseModel
                {
                    Id = p.Id,
                    EventId = p.EventId,
                    Price = p.Price,
                    UserId = p.UserId,
                    SeatIds = _purchasedSeatRepository.GetAll()
                        .Where(s => s.PurchaseId == p.Id)
                        .Select(s => s.EventSeatId)
                        .ToList(),
                };

                models.Add(purchaseModel);
            }

            return models;
        }

        public IEnumerable<EventSeatModel> GetByPurchaseId(int purchaseId)
        {
            List<int> purchasedSeatIds = _purchasedSeatRepository.GetAll()
                .Where(ps => ps.PurchaseId == purchaseId)
                .Select(ps => ps.EventSeatId)
                .ToList();

            List<EventSeatModel> eventSeats = _eventSeatService.GetAll()
                .Where(s => purchasedSeatIds.Contains(s.Id))
                .ToList();

            return eventSeats;
        }

        private async Task CheckSeatsAsync(IEnumerable<int> seatIds)
        {
            if (!seatIds.Any())
            {
                throw new ValidationException($"No seats chosen.");
            }

            foreach (int id in seatIds)
            {
                EventSeatModel seat = await _eventSeatService.GetByIdAsync(id);

                if (seat.State != EventSeatStateModel.Available)
                {
                    throw new ValidationException($"One or more seats have already been ordered.");
                }
            }
        }

        private async Task<decimal> CalculatePriceAsync(IEnumerable<int> seatIds)
        {
            decimal totalPrice = 0;

            foreach (int id in seatIds)
            {
                EventSeatModel seat = await _eventSeatService.GetByIdAsync(id);
                EventAreaModel area = await _eventAreaService.GetByIdAsync(seat.EventAreaId);

                totalPrice += area.Price;
            }

            return totalPrice;
        }

        private async Task MakePayment(string userId, decimal price)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ValidationException("User was not found.");
            }

            if (price > user.Balance)
            {
                throw new ValidationException("Your account has insufficient funds.");
            }

            user.Balance -= price;
        }
    }
}
