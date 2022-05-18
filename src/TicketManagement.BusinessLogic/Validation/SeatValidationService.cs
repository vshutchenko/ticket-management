using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class SeatValidationService : IValidationService<Seat>
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly List<ValidationDetails> _details;

        public SeatValidationService(IRepository<Seat> seatRepository)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Seat item)
        {
            return IsUniqueRowAndNumberInArea(item.AreaId, item.Row, item.Number);
        }

        private bool IsUniqueRowAndNumberInArea(int areaId, int row, int number)
        {
            if (_seatRepository.GetAll().Any(s => s.AreaId == areaId && s.Row == row && s.Number == number))
            {
                _details.Add(new ValidationDetails("seat with same row and number is already exists in the area", nameof(areaId), areaId.ToString()));
                return false;
            }

            return true;
        }
    }
}
