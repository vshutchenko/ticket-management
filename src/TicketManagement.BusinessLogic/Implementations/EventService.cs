using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Event> _eventValidator;

        public EventService(IRepository<Event> eventRepository, IValidator<Event> eventValidator)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
        }

        public Task<int> CreateAsync(Event @event)
        {
            if (@event is null)
            {
                throw new ValidationException("Event is null.");
            }

            @event.Id = 0;

            _eventValidator.Validate(@event);

            return _eventRepository.CreateAsync(@event);
        }

        public async Task DeleteAsync(int id)
        {
            await _eventRepository.CheckIfIdExistsAsync(id);

            await _eventRepository.DeleteAsync(id);
        }

        public IEnumerable<Event> GetAll()
        {
            return _eventRepository.GetAll();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            await _eventRepository.CheckIfIdExistsAsync(id);

            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Event @event)
        {
            if (@event is null)
            {
                throw new ValidationException("Event is null.");
            }

            await _eventRepository.CheckIfIdExistsAsync(@event.Id);

            _eventValidator.Validate(@event);

            await _eventRepository.UpdateAsync(@event);
        }
    }
}
