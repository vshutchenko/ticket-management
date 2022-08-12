using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.Services.Implementations
{
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IValidator<Layout> _layoutValidator;
        private readonly IMapper _mapper;

        public LayoutService(IRepository<Layout> layoutRepository, IValidator<Layout> layoutValidator, IMapper mapper)
        {
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _layoutValidator = layoutValidator ?? throw new ArgumentNullException(nameof(layoutValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(LayoutModel layoutModel)
        {
            if (layoutModel is null)
            {
                throw new ValidationException("Layout is null.");
            }

            var layout = _mapper.Map<Layout>(layoutModel);

            _layoutValidator.Validate(layout);

            return _layoutRepository.CreateAsync(layout);
        }

        public async Task DeleteAsync(int id)
        {
            await ValidateLayoutExistsAsync(id);

            await _layoutRepository.DeleteAsync(id);
        }

        public IEnumerable<LayoutModel> GetAll()
        {
            var models = _layoutRepository.GetAll().Select(l => _mapper.Map<LayoutModel>(l));

            return models;
        }

        public async Task<LayoutModel> GetByIdAsync(int id)
        {
            await ValidateLayoutExistsAsync(id);

            var layout = await _layoutRepository.GetByIdAsync(id);

            var model = _mapper.Map<LayoutModel>(layout);

            return model;
        }

        public async Task UpdateAsync(LayoutModel layoutModel)
        {
            if (layoutModel is null)
            {
                throw new ValidationException("Layout is null.");
            }

            await ValidateLayoutExistsAsync(layoutModel.Id);

            var layout = _mapper.Map<Layout>(layoutModel);

            _layoutValidator.Validate(layout);

            await _layoutRepository.UpdateAsync(layout);
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
