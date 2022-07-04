using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class SeatService : ISeatService
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly IValidator<Seat> _seatValidator;
        private readonly IMapper _mapper;

        public SeatService(IRepository<Seat> seatRepository, IValidator<Seat> seatValidator, IMapper mapper)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _seatValidator = seatValidator ?? throw new ArgumentNullException(nameof(seatValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(SeatModel seatModel)
        {
            if (seatModel is null)
            {
                throw new ValidationException("Seat is null.");
            }

            seatModel.Id = 0;
            var seat = _mapper.Map<Seat>(seatModel);

            _seatValidator.Validate(seat);

            return _seatRepository.CreateAsync(seat);
        }

        public async Task DeleteAsync(int id)
        {
            await _seatRepository.CheckIfIdExistsAsync(id);

            await _seatRepository.DeleteAsync(id);
        }

        public IEnumerable<SeatModel> GetAll()
        {
            var models = _seatRepository.GetAll().Select(s => _mapper.Map<SeatModel>(s));

            return models;
        }

        public async Task<SeatModel> GetByIdAsync(int id)
        {
            await _seatRepository.CheckIfIdExistsAsync(id);

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

            await _seatRepository.CheckIfIdExistsAsync(seatModel.Id);

            var seat = _mapper.Map<Seat>(seatModel);

            _seatValidator.Validate(seat);

            await _seatRepository.UpdateAsync(seat);
        }
    }
}
