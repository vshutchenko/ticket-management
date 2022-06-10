using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

[assembly: InternalsVisibleTo("TicketManagement.IntegrationTests")]
[assembly: InternalsVisibleTo("TicketManagement.UnitTests")]

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class AreaService : IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IValidator<Area> _areaValidator;

        public AreaService(IRepository<Area> areaRepository, IValidator<Area> areaValidator)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _areaValidator = areaValidator ?? throw new ArgumentNullException(nameof(areaValidator));
        }

        public int Create(Area area)
        {
            if (area is null)
            {
                throw new ValidationException("Area is null.");
            }

            area.Id = 0;

            _areaValidator.Validate(area);

            return _areaRepository.Create(area);
        }

        public void Delete(int id)
        {
            _areaRepository.CheckIfIdExists(id);

            _areaRepository.Delete(id);
        }

        public IEnumerable<Area> GetAll()
        {
            return _areaRepository.GetAll();
        }

        public Area GetById(int id)
        {
            _areaRepository.CheckIfIdExists(id);

            return _areaRepository.GetById(id);
        }

        public void Update(Area area)
        {
            if (area is null)
            {
                throw new ValidationException("Area is null.");
            }

            _areaRepository.CheckIfIdExists(area.Id);

            _areaValidator.Validate(area);

            _areaRepository.Update(area);
        }
    }
}
