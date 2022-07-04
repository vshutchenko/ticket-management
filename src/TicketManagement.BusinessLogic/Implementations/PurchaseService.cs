using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IMapper _mapper;

        public PurchaseService(
            UserManager<User> userManager,
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchasedSeat> purchasedSeatRepository,
            IEventSeatService eventSeatService,
            IRepository<EventArea> eventAreaRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _purchaseRepository = purchaseRepository;
            _purchasedSeatRepository = purchasedSeatRepository;
            _eventSeatService = eventSeatService;
            _eventAreaRepository = eventAreaRepository;
            _mapper = mapper;
        }

        public async Task PurchaseSeatAsync(PurchaseModel model)
        {
            await CheckSeatsAsync(model.SeatIds);

            model.Price = await CalculatePriceAsync(model.SeatIds);

            var purchase = _mapper.Map<Purchase>(model);

            await MakePayment(purchase.UserId, purchase.Price);

            var purchaseId = await _purchaseRepository.CreateAsync(purchase);

            foreach (var id in model.SeatIds)
            {
                await _eventSeatService.SetSeatStateAsync(id, EventSeatStateModel.Ordered);

                var purchasedSeat = new PurchasedSeat
                {
                    EventSeatId = id,
                    PurchaseId = purchaseId,
                };

                await _purchasedSeatRepository.CreateAsync(purchasedSeat);
            }
        }

        public IEnumerable<PurchaseModel> GetByUserId(string userId)
        {
            var purchases = _purchaseRepository.GetAll().Where(p => p.UserId == userId).ToList();

            List<PurchaseModel> models = new List<PurchaseModel>();

            foreach (var p in purchases)
            {
                var purchaseModel = _mapper.Map<PurchaseModel>(p);
                purchaseModel.SeatIds = _purchasedSeatRepository.GetAll()
                    .Where(s => s.PurchaseId == p.Id)
                    .Select(s => s.Id).ToList();

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

            var eventSeats = _eventSeatService.GetAll()
                .Where(s => purchasedSeatIds.Contains(s.Id))
                .Select(s => _mapper.Map<EventSeatModel>(s))
                .ToList();

            return eventSeats;
        }

        private async Task CheckSeatsAsync(IEnumerable<int> seatIds)
        {
            foreach (var id in seatIds)
            {
                var seat = await _eventSeatService.GetByIdAsync(id);
                var area = await _eventAreaRepository.GetByIdAsync(seat.EventAreaId);

                if (seat.State != EventSeatStateModel.Available)
                {
                    throw new ValidationException($"Seat {seat.Number} in the {seat.Row} row in the {area.Description} is already ordered.");
                }
            }
        }

        private async Task<decimal> CalculatePriceAsync(IEnumerable<int> seatIds)
        {
            decimal totalPrice = 0;

            foreach (var id in seatIds)
            {
                var seat = await _eventSeatService.GetByIdAsync(id);
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
