using AutoMapper;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Services.Implementations
{
    internal class AreaService : IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IValidator<Area> _areaValidator;
        private readonly IMapper _mapper;

        public AreaService(IRepository<Area> areaRepository, IRepository<Layout> layoutRepository, IValidator<Area> areaValidator, IMapper mapper)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
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

        public async Task<IEnumerable<AreaModel>> GetByLayoutIdAsync(int layoutId)
        {
            await ValidateLayoutExistsAsync(layoutId);

            var areas = _areaRepository.GetAll()
                .Where(a => a.LayoutId == layoutId)
                .Select(a => _mapper.Map<AreaModel>(a))
                .ToList();

            return areas;
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

        private async Task ValidateLayoutExistsAsync(int id)
        {
            var layout = await _layoutRepository.GetByIdAsync(id);

            if (layout is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
