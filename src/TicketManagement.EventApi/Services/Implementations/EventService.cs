using AutoMapper;
using TicketManagement.Core.Models;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.Services.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Event> _eventValidator;
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IMapper _mapper;

        public EventService(IRepository<Event> eventRepository, IValidator<Event> eventValidator, IEventSeatService eventSeatService, IEventAreaService eventAreaService, IMapper mapper)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
            _eventAreaService = eventAreaService ?? throw new ArgumentNullException(nameof(eventAreaService));
            _eventSeatService = eventSeatService ?? throw new ArgumentNullException(nameof(eventSeatService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(EventModel eventModel)
        {
            if (eventModel is null)
            {
                throw new ValidationException("Event is null.");
            }

            var @event = _mapper.Map<Event>(eventModel);

            _eventValidator.Validate(@event);

            return _eventRepository.CreateAsync(@event);
        }

        public async Task DeleteAsync(int id)
        {
            await ValidateEventExistsAsync(id);

            var orderedSeatsExist = _eventAreaService
                .GetByEventId(id)
                .SelectMany(a => _eventSeatService.GetByEventAreaId(a.Id))
                .Any(s => s.State == EventSeatState.Ordered);

            if (orderedSeatsExist)
            {
                throw new ValidationException("Event cannot be deleted because some seats has already been purchased.");
            }

            await _eventRepository.DeleteAsync(id);
        }

        public IEnumerable<EventModel> GetNotPublishedEvents()
        {
            var models = _eventRepository.GetAll()
                .Where(e => !e.Published)
                .Select(e => _mapper.Map<EventModel>(e))
                .ToList();

            return models;
        }

        public IEnumerable<EventModel> GetPublishedEvents()
        {
            var models = _eventRepository.GetAll()
                .Where(e => e.Published)
                .Select(e => _mapper.Map<EventModel>(e))
                .ToList();

            return models;
        }

        public async Task<EventModel> GetByIdAsync(int id)
        {
            await ValidateEventExistsAsync(id);

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

            var orderedSeatsExist = _eventAreaService
                .GetByEventId(eventModel.Id)
                .SelectMany(a => _eventSeatService.GetByEventAreaId(a.Id))
                .Any(s => s.State == EventSeatState.Ordered);

            if (orderedSeatsExist)
            {
                throw new ValidationException("Some seats have already been ordered.");
            }

            await ValidateEventExistsAsync(eventModel.Id);

            var @event = _mapper.Map<Event>(eventModel);

            _eventValidator.Validate(@event);

            await _eventRepository.UpdateAsync(@event);
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
