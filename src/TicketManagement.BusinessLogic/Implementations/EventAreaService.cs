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

        public async Task<EventArea> GetByIdAsync(int id)
        {
            await _eventAreaRepository.CheckIfIdExistsAsync(id);

            return await _eventAreaRepository.GetByIdAsync(id);
        }

        public async Task SetPriceAsync(int id, decimal price)
        {
            await _eventAreaRepository.CheckIfIdExistsAsync(id);

            _priceValidator.Validate(price);

            var area = await _eventAreaRepository.GetByIdAsync(id);

            area.Price = price;
            await _eventAreaRepository.UpdateAsync(area);
        }
    }
}
