using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventAreaService : IEventAreaService
    {
        private readonly IRepository<EventArea> _eventAreaRepository;

        public EventAreaService(IRepository<EventArea> eventAreaRepository)
        {
            _eventAreaRepository = eventAreaRepository ?? throw new ArgumentNullException(nameof(eventAreaRepository));
        }

        public IEnumerable<EventArea> GetAll()
        {
            return _eventAreaRepository.GetAll();
        }

        public EventArea GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _eventAreaRepository.GetById(id);
        }

        public void SetPrice(int id, decimal price)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            if (price < 0)
            {
                throw new ArgumentException("price cannot be negative");
            }

            var area = _eventAreaRepository.GetById(id);
            area.Price = price;
            _eventAreaRepository.Update(area);
        }
    }
}
