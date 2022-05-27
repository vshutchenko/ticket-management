using System;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class VenueValidator : IValidator<Venue>
    {
        private readonly IRepository<Venue> _venueRepository;

        public VenueValidator(IRepository<Venue> venueRepository)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
        }

        public void Validate(Venue item)
        {
            if (item is null)
            {
                throw new ValidationException("Venue is null.");
            }

            if (_venueRepository.GetAll().Any(v => v.Description.Equals(item.Description, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Venue with same description is already exists.");
            }
        }
    }
}
