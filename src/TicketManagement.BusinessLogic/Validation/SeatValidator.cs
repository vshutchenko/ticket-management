using System;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class SeatValidator : IValidator<Seat>
    {
        private readonly IRepository<Seat> _seatRepository;

        public SeatValidator(IRepository<Seat> seatRepository)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
        }

        public void Validate(Seat item)
        {
            if (item is null)
            {
                throw new ValidationException("Seat is null.");
            }

            bool seatExists = _seatRepository.GetAll().Any(s => s.Id != item.Id && s.AreaId == item.AreaId && s.Row == item.Row && s.Number == item.Number);

            if (seatExists)
            {
                throw new ValidationException("Seat with same row and number is already exists in the area.");
            }
        }
    }
}
