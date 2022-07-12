using System;
using System.Linq;
using TicketManagement.BusinessLogic.Extensions;
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

            var venueExists = _venueRepository.GetAll().Any(v => v.Id != item.Id && v.Description.Equals(item.Description, StringComparison.OrdinalIgnoreCase));

            if (venueExists)
            {
                throw new ValidationException("Venue with same description is already exists.");
            }
        }
    }
}
