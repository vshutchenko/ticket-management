using System;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class LayoutValidator : IValidator<Layout>
    {
        private readonly IRepository<Layout> _layoutRepsitory;

        public LayoutValidator(IRepository<Layout> layoutRepsitory)
        {
            _layoutRepsitory = layoutRepsitory ?? throw new ArgumentNullException(nameof(layoutRepsitory));
        }

        public void Validate(Layout item)
        {
            if (item is null)
            {
                throw new ValidationException("Layout is null.");
            }

            bool layoutExists = _layoutRepsitory
                .GetAll()
                .Any(l => l.VenueId == item.VenueId && item.Description.Equals(l.Description, StringComparison.OrdinalIgnoreCase));

            if (layoutExists)
            {
                throw new ValidationException("The same layout is already exists in current venue.");
            }
        }
    }
}
