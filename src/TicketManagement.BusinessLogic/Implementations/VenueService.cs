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
        private readonly IValidator<Venue> _venueValidator;

        public VenueService(IRepository<Venue> venueRepository, IValidator<Venue> venueValidator)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _venueValidator = venueValidator ?? throw new ArgumentNullException(nameof(venueValidator));
        }

        public int Create(Venue venue)
        {
            _venueValidator.Validate(venue);

            return _venueRepository.Create(venue);
        }

        public void Delete(int id)
        {
            _venueRepository.Delete(id);
        }

        public IEnumerable<Venue> GetAll()
        {
            return _venueRepository.GetAll();
        }

        public Venue GetById(int id)
        {
            return _venueRepository.GetById(id);
        }

        public void Update(Venue venue)
        {
            _venueValidator.Validate(venue);

            _venueRepository.Update(venue);
        }
    }
}
