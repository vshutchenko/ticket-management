using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Event> _validationService;

        public EventService(IRepository<Event> eventRepository, IValidator<Event> validationService)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public int Create(Event @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            _validationService.Validate(@event);

            return _eventRepository.Create(@event);
        }

        public void Delete(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            _eventRepository.Delete(id);
        }

        public IEnumerable<Event> GetAll()
        {
            return _eventRepository.GetAll();
        }

        public Event GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _eventRepository.GetById(id);
        }

        public void Update(Event @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            _validationService.Validate(@event);

            _eventRepository.Update(@event);
        }
    }
}
