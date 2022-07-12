using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IValidator<Venue> _venueValidator;
        private readonly IMapper _mapper;

        public VenueService(IRepository<Venue> venueRepository, IValidator<Venue> venueValidator, IMapper mapper)
        {
            _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
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
    }
}
