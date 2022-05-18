using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class LayoutValidationService : IValidationService<Layout>
    {
        private readonly IRepository<Layout> _layoutRepsitory;
        private readonly List<ValidationDetails> _details;

        public LayoutValidationService(IRepository<Layout> layoutRepsitory)
        {
            _layoutRepsitory = layoutRepsitory ?? throw new ArgumentNullException(nameof(layoutRepsitory));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Layout item)
        {
            return IsUniqueDescriptionInVenue(item.VenueId, item.Description);
        }

        private bool IsUniqueDescriptionInVenue(int venueId, string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (_layoutRepsitory.GetAll().Any(l => l.VenueId == venueId && description.Equals(l.Description, comparisonType)))
            {
                _details.Add(new ValidationDetails("the same layout is already exists int current venue", nameof(description), description.ToString()));
                return false;
            }

            return true;
        }
    }
}
