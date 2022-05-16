using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class LayoutValidationService : IValidationService<Layout>
    {
        private readonly ILayoutService _layoutService;
        private readonly List<ValidationDetails> _details;

        public LayoutValidationService(ILayoutService layoutService)
        {
            _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));

            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Layout item)
        {
            return IsUniqueDescriptionInVenue(item.VenueId, item.Description);
        }

        private bool IsUniqueDescriptionInVenue(int venueId, string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (_layoutService.GetAll().Any(l => l.VenueId == venueId && description.Equals(l.Description, comparisonType)))
            {
                _details.Add(new ValidationDetails("the same layout is already exists int current venue", nameof(description), description.ToString()));
                return false;
            }

            return true;
        }
    }
}
