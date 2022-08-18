using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.PurchaseApi.Models;
using TicketManagement.PurchaseApi.Services.Interfaces;
using TicketManagement.PurchaseApi.Services.Validation;

namespace TicketManagement.PurchaseApi.Services.Implementations
{
    internal class PurchaseService : IPurchaseService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<PurchasedSeat> _purchasedSeatRepository;
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IMapper _mapper;

        public PurchaseService(
            UserManager<User> userManager,
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchasedSeat> purchasedSeatRepository,
            IRepository<EventSeat> eventSeatRepository,
            IRepository<EventArea> eventAreaRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _purchaseRepository = purchaseRepository;
            _purchasedSeatRepository = purchasedSeatRepository;
            _eventSeatRepository = eventSeatRepository;
            _eventAreaRepository = eventAreaRepository;
            _mapper = mapper;
        }

        public async Task PurchaseSeatAsync(PurchaseModel model)
        {
            await CheckSeatsAsync(model.SeatIds);

            var purchase = new Purchase
            {
                EventId = model.EventId,
                UserId = model.UserId,
                Price = await CalculatePriceAsync(model.SeatIds),
            };

            await MakePayment(purchase.UserId!, purchase.Price);

            var purchaseId = await _purchaseRepository.CreateAsync(purchase);

            foreach (var id in model.SeatIds)
            {
                var seat = await _eventSeatRepository.GetByIdAsync(id);

                seat.State = EventSeatState.Ordered;

                await _eventSeatRepository.UpdateAsync(seat);

                var purchasedSeat = new PurchasedSeat { EventSeatId = id, PurchaseId = purchaseId };

                await _purchasedSeatRepository.CreateAsync(purchasedSeat);
            }
        }

        public IEnumerable<PurchaseModel> GetByUserId(string userId)
        {
            var purchases = _purchaseRepository.GetAll().Where(p => p.UserId == userId).ToList();

            var models = new List<PurchaseModel>();

            foreach (var p in purchases)
            {
                var purchaseModel = new PurchaseModel
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
            var purchasedSeatIds = _purchasedSeatRepository.GetAll()
                .Where(ps => ps.PurchaseId == purchaseId)
                .Select(ps => ps.EventSeatId)
                .ToList();

            var eventSeats = _eventSeatRepository.GetAll()
                .Where(s => purchasedSeatIds.Contains(s.Id))
                .Select(m => _mapper.Map<EventSeatModel>(m))
                .ToList();

            return eventSeats;
        }

        private async Task CheckSeatsAsync(IEnumerable<int> seatIds)
        {
            if (!seatIds.Any())
            {
                throw new ValidationException($"No seats chosen.");
            }

            foreach (var id in seatIds)
            {
                var seat = await _eventSeatRepository.GetByIdAsync(id);

                if (seat.State != EventSeatState.Available)
                {
                    throw new ValidationException($"One or more seats have already been ordered.");
                }
            }
        }

        private async Task<decimal> CalculatePriceAsync(IEnumerable<int> seatIds)
        {
            decimal totalPrice = 0;

            foreach (var id in seatIds)
            {
                var seat = await _eventSeatRepository.GetByIdAsync(id);
                var area = await _eventAreaRepository.GetByIdAsync(seat.EventAreaId);

                totalPrice += area.Price;
            }

            return totalPrice;
        }

        private async Task MakePayment(string userId, decimal price)
        {
            var user = await _userManager.FindByIdAsync(userId);

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
