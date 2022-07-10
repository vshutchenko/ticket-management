using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventAreaService : IEventAreaService
    {
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<decimal> _priceValidator;
        private readonly IMapper _mapper;

        public EventAreaService(IRepository<EventArea> eventAreaRepository, IRepository<Event> eventRepository, IValidator<decimal> priceValidator, IMapper mapper)
        {
            _eventAreaRepository = eventAreaRepository ?? throw new ArgumentNullException(nameof(eventAreaRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _priceValidator = priceValidator ?? throw new ArgumentNullException(nameof(priceValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<EventAreaModel> GetAll()
        {
            var models = _eventAreaRepository.GetAll()
                .Select(a => _mapper.Map<EventAreaModel>(a))
                .ToList();

            return models;
        }

        public IEnumerable<EventAreaModel> GetByEventId(int eventId)
        {
            Task.Run(async () => await ValidateEventExistsAsync(eventId));

            var models = _eventAreaRepository.GetAll()
                .Where(a => a.EventId == eventId)
                .Select(a => _mapper.Map<EventAreaModel>(a))
                .ToList();

            return models;
        }

        public async Task<EventAreaModel> GetByIdAsync(int id)
        {
            await ValidateEventAreaExistsAsync(id);

            var eventArea = await _eventAreaRepository.GetByIdAsync(id);

            var model = _mapper.Map<EventAreaModel>(eventArea);

            return model;
        }

        public async Task SetPriceAsync(int id, decimal price)
        {
            await ValidateEventAreaExistsAsync(id);

            _priceValidator.Validate(price);

            var area = await _eventAreaRepository.GetByIdAsync(id);

            area.Price = price;
            await _eventAreaRepository.UpdateAsync(area);
        }

        private async Task ValidateEventAreaExistsAsync(int id)
        {
            var eventArea = await _eventAreaRepository.GetByIdAsync(id);

            if (eventArea is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }

        private async Task ValidateEventExistsAsync(int id)
        {
            var @event = await _eventRepository.GetByIdAsync(id);

            if (@event is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
