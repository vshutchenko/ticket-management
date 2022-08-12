using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.EventApi.Services.Validation
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

            var venueExists = _venueRepository
                .GetAll().AsEnumerable()
                .Any(v => v.Id != item.Id && v.Description.Equals(item.Description, StringComparison.OrdinalIgnoreCase));

            if (venueExists)
            {
                throw new ValidationException("Venue with same description is already exists.");
            }
        }
    }
}
