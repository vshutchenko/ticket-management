using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
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
        private readonly IMapper _mapper;

        public AreaService(IRepository<Area> areaRepository, IValidator<Area> areaValidator, IMapper mapper)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _areaValidator = areaValidator ?? throw new ArgumentNullException(nameof(areaValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(AreaModel areaModel)
        {
            if (areaModel is null)
            {
                throw new ValidationException("Area is null.");
            }

            areaModel.Id = 0;
            var area = _mapper.Map<Area>(areaModel);

            _areaValidator.Validate(area);

            return _areaRepository.CreateAsync(area);
        }

        public async Task DeleteAsync(int id)
        {
            await _areaRepository.CheckIfIdExistsAsync(id);

            await _areaRepository.DeleteAsync(id);
        }

        public IEnumerable<AreaModel> GetAll()
        {
            var models = _areaRepository.GetAll().Select(a => _mapper.Map<AreaModel>(a));

            return models;
        }

        public async Task<AreaModel> GetByIdAsync(int id)
        {
            await _areaRepository.CheckIfIdExistsAsync(id);

            var area = await _areaRepository.GetByIdAsync(id);

            var model = _mapper.Map<AreaModel>(area);

            return model;
        }

        public async Task UpdateAsync(AreaModel areaModel)
        {
            if (areaModel is null)
            {
                throw new ValidationException("Area is null.");
            }

            await _areaRepository.CheckIfIdExistsAsync(areaModel.Id);

            var area = _mapper.Map<Area>(areaModel);

            _areaValidator.Validate(area);

            await _areaRepository.UpdateAsync(area);
        }
    }
}
