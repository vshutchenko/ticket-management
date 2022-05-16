using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidationService<Event> _eventValidator;

        public EventService(IRepository<Event> eventRepository, IValidationService<Event> eventValidator)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
        }

        public int Create(Event @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (!_eventValidator.Validate(@event))
            {
                throw new ArgumentException(nameof(@event), "invalid event")
            }

            return _eventRepository.Create(@event);
        }

        public void Delete(int id)
        {
            _eventRepository.Delete(id);
        }

        public IEnumerable<Event> GetAll()
        {
            foreach (var @event in _eventRepository.Get(offset, limit))
            {
                yield return @event;
            }
        }

        public Event GetById(int id)
        {
            return _eventRepository.GetById(id);
        }

        public void Update(Event @event)
        {
            throw new NotImplementedException();
        }
    }
}
