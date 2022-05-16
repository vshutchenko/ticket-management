using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IValidationService<Venue> _validationService;

        public VenueService(IRepository<Venue> venueRepository, IValidationService<Venue> validationService)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public int Create(Venue venue)
        {
            if (venue is null)
            {
                throw new ArgumentNullException(nameof(venue));
            }

            bool isValid = _validationService.Validate(venue);

            if (isValid)
            {
                return _venueRepository.Create(venue);
            }

            return -1;
        }

        public void Delete(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            _venueRepository.Delete(id);
        }

        public IEnumerable<Venue> GetAll()
        {
            return _venueRepository.GetAll();
        }

        public Venue GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _venueRepository.GetById(id);
        }

        public void Update(Venue venue)
        {
            if (venue is null)
            {
                throw new ArgumentNullException(nameof(venue));
            }

            if (_validationService.Validate(venue))
            {
                _venueRepository.Update(venue);
            }
        }
    }
}
