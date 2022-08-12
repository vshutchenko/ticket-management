using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.Services.Implementations
{
    internal class EventSeatService : IEventSeatService
    {
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IMapper _mapper;

        public EventSeatService(IRepository<EventSeat> eventSeatRepository, IRepository<EventArea> eventAreaRepository, IMapper mapper)
        {
            _eventSeatRepository = eventSeatRepository ?? throw new ArgumentNullException(nameof(eventSeatRepository));
            _eventAreaRepository = eventAreaRepository ?? throw new ArgumentNullException(nameof(eventAreaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<EventSeatModel> GetAll()
        {
            var models = _eventSeatRepository.GetAll()
                .Select(s => _mapper.Map<EventSeatModel>(s))
                .ToList();

            return models;
        }

        public async Task<EventSeatModel> GetByIdAsync(int id)
        {
            await ValidateEventSeatExistsAsync(id);

            var seat = await _eventSeatRepository.GetByIdAsync(id);

            var model = _mapper.Map<EventSeatModel>(seat);

            return model;
        }

        public async Task SetSeatStateAsync(int id, EventSeatStateModel stateModel)
        {
            await ValidateEventSeatExistsAsync(id);

            var seat = await _eventSeatRepository.GetByIdAsync(id);

            var state = _mapper.Map<EventSeatState>(stateModel);

            seat.State = state;
            await _eventSeatRepository.UpdateAsync(seat);
        }

        public IEnumerable<EventSeatModel> GetByEventAreaId(int eventAreaId)
        {
            ValidateEventAreaExists(eventAreaId);

            var models = _eventSeatRepository.GetAll()
                .Where(s => s.EventAreaId == eventAreaId)
                .Select(s => _mapper.Map<EventSeatModel>(s))
                .ToList();

            return models;
        }

        private async Task ValidateEventSeatExistsAsync(int id)
        {
            var eventSeat = await _eventSeatRepository.GetByIdAsync(id);

            if (eventSeat is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }

        private void ValidateEventAreaExists(int id)
        {
            var eventArea = _eventAreaRepository.GetAll().FirstOrDefault(e => e.Id == id);

            if (eventArea is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
