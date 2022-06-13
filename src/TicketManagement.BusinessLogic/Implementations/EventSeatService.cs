using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
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

        public EventSeat GetById(int id)
        {
            _eventSeatRepository.CheckIfIdExists(id);

            return _eventSeatRepository.GetById(id);
        }

        public void SetSeatState(int id, EventSeatState state)
        {
            _eventSeatRepository.CheckIfIdExists(id);

            var seat = _eventSeatRepository.GetById(id);

            seat.State = state;
            _eventSeatRepository.Update(seat);
        }
    }
}
