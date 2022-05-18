using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class EventValidationService : IValidationService<Event>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly List<ValidationDetails> _details;

        public EventValidationService(IRepository<Event> eventRepository, IRepository<Seat> seatRepository, IRepository<Area> areaRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _details = new List<ValidationDetails>();
        }

        public IEnumerable<ValidationDetails> Details => _details;

        public bool Validate(Event item)
        {
            return IsValidDate(item.StartDate, item.EndDate)
                && IsAvaiableLayout(item.LayoutId, item.StartDate, item.EndDate)
                && IsAvaiableSeats(item.LayoutId);
        }

        private bool IsValidDate(DateTime start, DateTime end)
        {
            if (start < DateTime.Now)
            {
                _details.Add(new ValidationDetails("cannot create event in the past", nameof(start), start.ToString()));
                return false;
            }

            if (end < start)
            {
                _details.Add(new ValidationDetails("end date is less than start date", nameof(end), end.ToString()));
                return false;
            }

            return true;
        }

        private bool IsAvaiableLayout(int layoutId, DateTime start, DateTime end)
        {
            var eventInTheSameLayout = _eventRepository.GetAll().FirstOrDefault(e => e.LayoutId == layoutId);

            if (eventInTheSameLayout != null)
            {
                if (start.InRange(eventInTheSameLayout.StartDate, eventInTheSameLayout.EndDate)
                    || end.InRange(eventInTheSameLayout.StartDate, eventInTheSameLayout.EndDate))
                {
                    _details.Add(new ValidationDetails("event in the same layout and the same time is already exists",
                        nameof(eventInTheSameLayout.StartDate), eventInTheSameLayout.StartDate.ToString()));
                }

                return false;
            }

            return true;
        }

        private bool IsAvaiableSeats(int layoutId)
        {
            var layoutAreas = _areaRepository.GetAll().Where(a => a.LayoutId == layoutId).ToList();
            if (!_seatRepository.GetAll().Any(s => layoutAreas.Any(a => a.Id == s.AreaId)))
            {
                _details.Add(new ValidationDetails("cannot create event without available seats in layout", nameof(layoutId), layoutId.ToString()));
                return false;
            }

            return true;
        }
    }
}
