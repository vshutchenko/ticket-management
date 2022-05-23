using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private readonly IValidator<Area> _validationService;

        public AreaService(IRepository<Area> areaRepository, IValidator<Area> validationService)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public int Create(Area area)
        {
            if (area is null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            _validationService.Validate(area);

            return _areaRepository.Create(area);
        }

        public void Delete(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            _areaRepository.Delete(id);
        }

        public IEnumerable<Area> GetAll()
        {
            return _areaRepository.GetAll();
        }

        public Area GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _areaRepository.GetById(id);
        }

        public void Update(Area area)
        {
            if (area is null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            _validationService.Validate(area);

            _areaRepository.Update(area);
        }
    }
}
