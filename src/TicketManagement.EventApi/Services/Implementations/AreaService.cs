using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.Services.Implementations
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

            var area = _mapper.Map<Area>(areaModel);

            _areaValidator.Validate(area);

            return _areaRepository.CreateAsync(area);
        }

        public async Task DeleteAsync(int id)
        {
            await ValidateAreaExistsAsync(id);

            await _areaRepository.DeleteAsync(id);
        }

        public IEnumerable<AreaModel> GetAll()
        {
            var models = _areaRepository.GetAll().Select(a => _mapper.Map<AreaModel>(a));

            return models;
        }

        public async Task<AreaModel> GetByIdAsync(int id)
        {
            await ValidateAreaExistsAsync(id);

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

            await ValidateAreaExistsAsync(areaModel.Id);

            var area = _mapper.Map<Area>(areaModel);

            _areaValidator.Validate(area);

            await _areaRepository.UpdateAsync(area);
        }

        private async Task ValidateAreaExistsAsync(int id)
        {
            var area = await _areaRepository.GetByIdAsync(id);

            if (area is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
