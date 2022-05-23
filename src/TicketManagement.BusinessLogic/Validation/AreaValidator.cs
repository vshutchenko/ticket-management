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

        public bool Validate(Area item)
        {
            if (_areaRepository.GetAll().Any(a => a.LayoutId == item.LayoutId && a.Description.Equals(item.Description, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Area description should be unique in the layout.");
            }

            return true;
        }
    }
}
