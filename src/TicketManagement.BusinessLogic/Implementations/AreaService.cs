using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public Task<int> CreateAsync(Area area)
        {
            if (area is null)
            {
                throw new ValidationException("Area is null.");
            }

            area.Id = 0;

            _areaValidator.Validate(area);

            return _areaRepository.CreateAsync(area);
        }

        public async Task DeleteAsync(int id)
        {
            await _areaRepository.CheckIfIdExistsAsync(id);

            await _areaRepository.DeleteAsync(id);
        }

        public IEnumerable<Area> GetAll()
        {
            return _areaRepository.GetAll();
        }

        public async Task<Area> GetByIdAsync(int id)
        {
            await _areaRepository.CheckIfIdExistsAsync(id);

            return await _areaRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Area area)
        {
            if (area is null)
            {
                throw new ValidationException("Area is null.");
            }

            await _areaRepository.CheckIfIdExistsAsync(area.Id);

            _areaValidator.Validate(area);

            await _areaRepository.UpdateAsync(area);
        }
    }
}
