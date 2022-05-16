using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class VenueValidationService : IValidationService<Venue>
    {
        private readonly IVenueService _venueService;
        private readonly List<ValidationDetails> _details;

        public VenueValidationService(IVenueService venueService)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Venue item)
        {
            return IsUniqueDecription(item.Description);
        }

        private bool IsUniqueDecription(string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (_venueService.GetAll().Any(v => v.Description.Equals(description, comparisonType)))
            {
                _details.Add(new ValidationDetails("the same venue is already exists", nameof(description), description.ToString()));
                return false;
            }

            return true;
        }
    }
}
