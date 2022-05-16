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
        private readonly IValidationService<Seat> _validationService;

        public SeatService(IRepository<Seat> seatRepository, IValidationService<Seat> validationService)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public int Create(Seat seat)
        {
            if (seat is null)
            {
                throw new ArgumentNullException(nameof(seat));
            }

            bool isValid = _validationService.Validate(seat);

            if (isValid)
            {
                return _seatRepository.Create(seat);
            }

            return -1;
        }

        public void Delete(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            _seatRepository.Delete(id);
        }

        public IEnumerable<Seat> GetAll()
        {
            return _seatRepository.GetAll();
        }

        public Seat GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _seatRepository.GetById(id);
        }

        public void Update(Seat seat)
        {
            if (seat is null)
            {
                throw new ArgumentNullException(nameof(seat));
            }

            if (_validationService.Validate(seat))
            {
                _seatRepository.Update(seat);
            }
        }
    }
}
