using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class VenueValidationService : IValidationService<Venue>
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly List<ValidationDetails> _details;

        public VenueValidationService(IRepository<Venue> venueRepository)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Venue item)
        {
            return IsUniqueDecription(item.Description);
        }

        private bool IsUniqueDecription(string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (_venueRepository.GetAll().Any(v => v.Description.Equals(description, comparisonType)))
            {
                _details.Add(new ValidationDetails("the same venue is already exists", nameof(description), description.ToString()));
                return false;
            }

            return true;
        }
    }
}
