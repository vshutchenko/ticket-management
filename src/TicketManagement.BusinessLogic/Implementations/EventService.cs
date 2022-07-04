using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Event> _eventValidator;
        private readonly IMapper _mapper;

        public EventService(IRepository<Event> eventRepository, IValidator<Event> eventValidator, IMapper mapper)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(EventModel eventModel)
        {
            if (eventModel is null)
            {
                throw new ValidationException("Event is null.");
            }

            eventModel.Id = 0;

            var @event = _mapper.Map<Event>(eventModel);

            _eventValidator.Validate(@event);

            return _eventRepository.CreateAsync(@event);
        }

        public async Task DeleteAsync(int id)
        {
            await _eventRepository.CheckIfIdExistsAsync(id);

            await _eventRepository.DeleteAsync(id);
        }

        public IEnumerable<EventModel> GetAll()
        {
            var models = _eventRepository.GetAll()
                .Select(e => _mapper.Map<EventModel>(e))
                .ToList();

            return models;
        }

        public async Task<EventModel> GetByIdAsync(int id)
        {
            await _eventRepository.CheckIfIdExistsAsync(id);

            var @event = await _eventRepository.GetByIdAsync(id);

            var model = _mapper.Map<EventModel>(@event);

            return model;
        }

        public async Task UpdateAsync(EventModel eventModel)
        {
            if (eventModel is null)
            {
                throw new ValidationException("Event is null.");
            }

            await _eventRepository.CheckIfIdExistsAsync(eventModel.Id);

            var @event = _mapper.Map<Event>(eventModel);

            _eventValidator.Validate(@event);

            await _eventRepository.UpdateAsync(@event);
        }

        public int Count()
        {
            return _eventRepository.GetAll().Count();
        }

        public IEnumerable<EventModel> Get(int limit, int offset)
        {
            var models = _eventRepository.GetAll()
                .Take(limit).Skip(offset)
                .Select(e => _mapper.Map<EventModel>(e))
                .ToList();

            return models;
        }
    }
}
