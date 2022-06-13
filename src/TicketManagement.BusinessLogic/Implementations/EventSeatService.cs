using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventSeatService : IEventSeatService
    {
        private readonly IRepository<EventSeat> _eventSeatRepository;

        public EventSeatService(IRepository<EventSeat> eventSeatRepository)
        {
            _eventSeatRepository = eventSeatRepository ?? throw new ArgumentNullException(nameof(eventSeatRepository));
        }

        public IEnumerable<EventSeat> GetAll()
        {
            return _eventSeatRepository.GetAll();
        }

        public async Task<EventSeat> GetByIdAsync(int id)
        {
            await _eventSeatRepository.CheckIfIdExistsAsync(id);

            return await _eventSeatRepository.GetByIdAsync(id);
        }

        public async Task SetSeatStateAsync(int id, EventSeatState state)
        {
            await _eventSeatRepository.CheckIfIdExistsAsync(id);

            var seat = await _eventSeatRepository.GetByIdAsync(id);

            seat.State = state;
            await _eventSeatRepository.UpdateAsync(seat);
        }
    }
}
