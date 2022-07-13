using System;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class AreaValidator : IValidator<Area>
    {
        private readonly IRepository<Area> _areaRepository;

        public AreaValidator(IRepository<Area> areaRepository)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
        }

        public void Validate(Area item)
        {
            if (item is null)
            {
                throw new ValidationException("Area is null.");
            }

            var areaExists = _areaRepository
                .GetAll().AsEnumerable()
                .Any(a => a.Id != item.Id && a.LayoutId == item.LayoutId && a.Description.Equals(item.Description, StringComparison.OrdinalIgnoreCase));

            if (areaExists)
            {
                throw new ValidationException("Area description should be unique in the layout.");
            }
        }
    }
}
