using AutoMapper;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;

namespace TicketManagement.VenueApi.Services.Implementations
{
    internal class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Venue> _venueValidator;

        private readonly IMapper _mapper;

        public VenueService(IRepository<Venue> venueRepository, IRepository<Layout> layoutRepository, IRepository<Event> eventRepository, IValidator<Venue> venueValidator, IMapper mapper)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _venueValidator = venueValidator ?? throw new ArgumentNullException(nameof(venueValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(VenueModel venueModel)
        {
            if (venueModel is null)
            {
                throw new ValidationException("Venue is null.");
            }

            var venue = _mapper.Map<Venue>(venueModel);

            _venueValidator.Validate(venue);

            return _venueRepository.CreateAsync(venue);
        }

        public async Task DeleteAsync(int id)
        {
            await ValidateVenueExistsAsync(id);

            ValidateNoEventsInVenue(id);

            await _venueRepository.DeleteAsync(id);
        }

        public IEnumerable<VenueModel> GetAll()
        {
            var models = _venueRepository.GetAll().Select(v => _mapper.Map<VenueModel>(v));

            return models;
        }

        public async Task<VenueModel> GetByIdAsync(int id)
        {
            await ValidateVenueExistsAsync(id);

            var venue = await _venueRepository.GetByIdAsync(id);

            var model = _mapper.Map<VenueModel>(venue);

            return model;
        }

        public async Task UpdateAsync(VenueModel venueModel)
        {
            if (venueModel is null)
            {
                throw new ValidationException("Venue is null.");
            }

            await ValidateVenueExistsAsync(venueModel.Id);

            var venue = _mapper.Map<Venue>(venueModel);

            _venueValidator.Validate(venue);

            await _venueRepository.UpdateAsync(venue);
        }

        private async Task ValidateVenueExistsAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }

        private void ValidateNoEventsInVenue(int venueId)
        {
            var layoutsInVenue = _layoutRepository.GetAll()
                .Where(l => l.VenueId == venueId)
                .ToList();

            foreach (var layout in layoutsInVenue)
            {
                var eventInVenue = _eventRepository.GetAll()
                    .FirstOrDefault(e => e.LayoutId == layout.Id);

                if (eventInVenue is not null)
                {
                    throw new ValidationException("This venue cannot be deleted as it will host an event.");
                }
            }
        }
    }
}
