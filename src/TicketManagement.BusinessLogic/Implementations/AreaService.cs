using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class AreaService : IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IValidationService<Area> _validationService;

        public AreaService(IRepository<Area> areaRepository, IValidationService<Area> validationService)
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

            bool isValid = _validationService.Validate(area);

            if (isValid)
            {
                return _areaRepository.Create(area);
            }

            return -1;
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

        public void Update(Area layout)
        {
            if (layout is null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            if (_validationService.Validate(layout))
            {
                _areaRepository.Update(layout);
            }
        }
    }
}
