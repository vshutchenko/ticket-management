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
    internal class SeatService : ISeatService
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly IValidator<Seat> _seatValidator;

        public SeatService(IRepository<Seat> seatRepository, IValidator<Seat> seatValidator)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _seatValidator = seatValidator ?? throw new ArgumentNullException(nameof(seatValidator));
        }

        public Task<int> CreateAsync(Seat seat)
        {
            if (seat is null)
            {
                throw new ValidationException("Seat is null.");
            }

            seat.Id = 0;

            _seatValidator.Validate(seat);

            return _seatRepository.CreateAsync(seat);
        }

        public async Task DeleteAsync(int id)
        {
            await _seatRepository.CheckIfIdExistsAsync(id);

            await _seatRepository.DeleteAsync(id);
        }

        public IEnumerable<Seat> GetAll()
        {
            return _seatRepository.GetAll();
        }

        public async Task<Seat> GetByIdAsync(int id)
        {
            await _seatRepository.CheckIfIdExistsAsync(id);

            return await _seatRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Seat seat)
        {
            if (seat is null)
            {
                throw new ValidationException("Seat is null.");
            }

            await _seatRepository.CheckIfIdExistsAsync(seat.Id);

            _seatValidator.Validate(seat);

            await _seatRepository.UpdateAsync(seat);
        }
    }
}
