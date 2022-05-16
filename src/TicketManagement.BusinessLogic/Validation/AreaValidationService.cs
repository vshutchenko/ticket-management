using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class AreaValidationService : IValidationService<Area>
    {
        private readonly IAreaService _areaService;
        private readonly List<ValidationDetails> _details;

        public AreaValidationService(IAreaService areaService)
        {
            _areaService = areaService ?? throw new ArgumentNullException(nameof(areaService));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Area item)
        {
            return IsUniqueDecriptionInLayout(item.LayoutId, item.Description);
        }

        private bool IsUniqueDecriptionInLayout(int layoutId, string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (_areaService.GetAll().Any(a => a.LayoutId == layoutId && a.Description.Equals(description, comparisonType)))
            {
                _details.Add(new ValidationDetails("the same area is already exists in the layout", nameof(description), description.ToString()));
                return false;
            }

            return true;
        }
    }
}
