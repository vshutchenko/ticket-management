using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventAreaService : IEventAreaService
    {
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IValidator<decimal> _priceValidator;

        public EventAreaService(IRepository<EventArea> eventAreaRepository, IValidator<decimal> priceValidator)
        {
            _eventAreaRepository = eventAreaRepository ?? throw new ArgumentNullException(nameof(eventAreaRepository));
            _priceValidator = priceValidator ?? throw new ArgumentNullException(nameof(priceValidator));
        }

        public IEnumerable<EventArea> GetAll()
        {
            return _eventAreaRepository.GetAll();
        }

        public EventArea GetById(int id)
        {
            return _eventAreaRepository.GetById(id);
        }

        public void SetPrice(int id, decimal price)
        {
            _priceValidator.Validate(price);

            var area = _eventAreaRepository.GetById(id);

            if (area is null)
            {
                throw new ValidationException("Event area does not exist.");
            }

            area.Price = price;
            _eventAreaRepository.Update(area);
        }
    }
}
