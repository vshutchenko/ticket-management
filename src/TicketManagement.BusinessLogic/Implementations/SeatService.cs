using System;
using System.Collections.Generic;
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

        public int Create(Seat seat)
        {
            if (seat is null)
            {
                throw new ValidationException("Seat is null.");
            }

            seat.Id = 0;

            _seatValidator.Validate(seat);

            return _seatRepository.Create(seat);
        }

        public void Delete(int id)
        {
            _seatRepository.CheckIfIdExists(id);

            _seatRepository.Delete(id);
        }

        public IEnumerable<Seat> GetAll()
        {
            return _seatRepository.GetAll();
        }

        public Seat GetById(int id)
        {
            _seatRepository.CheckIfIdExists(id);

            return _seatRepository.GetById(id);
        }

        public void Update(Seat seat)
        {
            if (seat is null)
            {
                throw new ValidationException("Seat is null.");
            }

            _seatRepository.CheckIfIdExists(seat.Id);

            _seatValidator.Validate(seat);

            _seatRepository.Update(seat);
        }
    }
}
