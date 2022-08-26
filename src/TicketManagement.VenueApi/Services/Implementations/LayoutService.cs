using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.VenueApi.Services.Implementations
{
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IRepository<Venue> _venueRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Layout> _layoutValidator;
        private readonly IMapper _mapper;

        public LayoutService(
            IRepository<Layout> layoutRepository,
            IRepository<Venue> venueRepository,
            IRepository<Event> eventRepository,
            IValidator<Layout> layoutValidator,
            IMapper mapper)
        {
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
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

            ValidateNoEventsInLayout(id);

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

        public async Task<IEnumerable<LayoutModel>> GetByVenueIdAsync(int venueId)
        {
            await ValidateVenueExistsAsync(venueId);

            var layouts = _layoutRepository.GetAll()
                .Where(l => l.VenueId == venueId)
                .Select(l => _mapper.Map<LayoutModel>(l))
                .ToList();

            return layouts;
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

        private async Task ValidateVenueExistsAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }

        private void ValidateNoEventsInLayout(int layoutId)
        {
            var eventInLayout = _eventRepository.GetAll()
                .FirstOrDefault(e => e.LayoutId == layoutId);

            if (eventInLayout is not null)
            {
                throw new ValidationException("This layout cannot be deleted as it will host an event.");
            }
        }
    }
}
