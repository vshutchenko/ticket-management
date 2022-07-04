using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventSeatService : IEventSeatService
    {
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IMapper _mapper;

        public EventSeatService(IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _eventSeatRepository = eventSeatRepository ?? throw new ArgumentNullException(nameof(eventSeatRepository));
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
            await _eventSeatRepository.CheckIfIdExistsAsync(id);

            var seat = await _eventSeatRepository.GetByIdAsync(id);

            var model = _mapper.Map<EventSeatModel>(seat);

            return model;
        }

        public async Task SetSeatStateAsync(int id, EventSeatStateModel stateModel)
        {
            await _eventSeatRepository.CheckIfIdExistsAsync(id);

            var seat = await _eventSeatRepository.GetByIdAsync(id);

            var state = _mapper.Map<EventSeatState>(stateModel);

            seat.State = state;
            await _eventSeatRepository.UpdateAsync(seat);
        }

        public IEnumerable<EventSeatModel> GetByEventAreaId(int eventAreaId)
        {
            // add throwing if area not found
            var models = _eventSeatRepository.GetAll()
                .Where(s => s.EventAreaId == eventAreaId)
                .Select(s => _mapper.Map<EventSeatModel>(s))
                .ToList();

            return models;
        }
    }
}
