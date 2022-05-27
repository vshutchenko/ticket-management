using System;
using System.Collections.Generic;
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
            _seatValidator.Validate(seat);

            return _seatRepository.Create(seat);
        }

        public void Delete(int id)
        {
            _seatRepository.Delete(id);
        }

        public IEnumerable<Seat> GetAll()
        {
            return _seatRepository.GetAll();
        }

        public Seat GetById(int id)
        {
            return _seatRepository.GetById(id);
        }

        public void Update(Seat seat)
        {
            _seatValidator.Validate(seat);
            _seatRepository.Update(seat);
        }
    }
}
