using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.VenueApi.Services.Implementations
{
    internal class SeatService : ISeatService
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IValidator<Seat> _seatValidator;
        private readonly IMapper _mapper;

        public SeatService(IRepository<Seat> seatRepository, IRepository<Area> areaRepository, IValidator<Seat> seatValidator, IMapper mapper)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _seatValidator = seatValidator ?? throw new ArgumentNullException(nameof(seatValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(SeatModel seatModel)
        {
            if (seatModel is null)
            {
                throw new ValidationException("Seat is null.");
            }

            var seat = _mapper.Map<Seat>(seatModel);

            _seatValidator.Validate(seat);

            return _seatRepository.CreateAsync(seat);
        }

        public async Task DeleteAsync(int id)
        {
            await ValidateSeatExistsAsync(id);

            await _seatRepository.DeleteAsync(id);
        }

        public IEnumerable<SeatModel> GetAll()
        {
            var models = _seatRepository.GetAll().Select(s => _mapper.Map<SeatModel>(s));

            return models;
        }

        public async Task<IEnumerable<SeatModel>> GetByAreaIdAsync(int areaId)
        {
            await ValidateAreaExistsAsync(areaId);

            var seats = _seatRepository.GetAll()
                .Where(s => s.AreaId == areaId)
                .Select(s => _mapper.Map<SeatModel>(s))
                .ToList();

            return seats;
        }

        public async Task<SeatModel> GetByIdAsync(int id)
        {
            await ValidateSeatExistsAsync(id);

            var seat = await _seatRepository.GetByIdAsync(id);

            var model = _mapper.Map<SeatModel>(seat);

            return model;
        }

        public async Task UpdateAsync(SeatModel seatModel)
        {
            if (seatModel is null)
            {
                throw new ValidationException("Seat is null.");
            }

            await ValidateSeatExistsAsync(seatModel.Id);

            var seat = _mapper.Map<Seat>(seatModel);

            _seatValidator.Validate(seat);

            await _seatRepository.UpdateAsync(seat);
        }

        private async Task ValidateSeatExistsAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat is null)
            {
                throw new ValidationException("Entity was not found.");
            }
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
