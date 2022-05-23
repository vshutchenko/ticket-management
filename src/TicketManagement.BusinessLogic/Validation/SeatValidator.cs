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

        public bool Validate(Seat item)
        {
            if (_seatRepository.GetAll().Any(s => s.AreaId == item.AreaId && s.Row == item.Row && s.Number == item.Number))
            {
                throw new ValidationException("Seat with same row and number is already exists in the area.");
            }

            return true;
        }
    }
}
