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
    internal class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IValidator<Venue> _venueValidator;

        public VenueService(IRepository<Venue> venueRepository, IValidator<Venue> venueValidator)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _venueValidator = venueValidator ?? throw new ArgumentNullException(nameof(venueValidator));
        }

        public Task<int> CreateAsync(Venue venue)
        {
            if (venue is null)
            {
                throw new ValidationException("Venue is null.");
            }

            venue.Id = 0;

            _venueValidator.Validate(venue);

            return _venueRepository.CreateAsync(venue);
        }

        public async Task DeleteAsync(int id)
        {
            await _venueRepository.CheckIfIdExistsAsync(id);

            await _venueRepository.DeleteAsync(id);
        }

        public IEnumerable<Venue> GetAll()
        {
            return _venueRepository.GetAll();
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            await _venueRepository.CheckIfIdExistsAsync(id);

            return await _venueRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Venue venue)
        {
            if (venue is null)
            {
                throw new ValidationException("Venue is null.");
            }

            await _venueRepository.CheckIfIdExistsAsync(venue.Id);

            _venueValidator.Validate(venue);

            await _venueRepository.UpdateAsync(venue);
        }
    }
}
