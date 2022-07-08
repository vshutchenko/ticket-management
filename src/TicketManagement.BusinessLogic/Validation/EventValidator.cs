using System;
using System.Linq;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class EventValidator : IValidator<Event>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IRepository<EventArea> _eventAreaRepository;

        public EventValidator(
            IRepository<Event> eventRepository,
            IRepository<Seat> seatRepository,
            IRepository<Area> areaRepository,
            IRepository<EventSeat> eventSeatRepository,
            IRepository<EventArea> eventAreaRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _eventSeatRepository = eventSeatRepository ?? throw new ArgumentNullException(nameof(eventSeatRepository));
            _eventAreaRepository = eventAreaRepository ?? throw new ArgumentNullException(nameof(eventAreaRepository));
        }

        public void Validate(Event item)
        {
            if (item is null)
            {
                throw new ValidationException("Event is null.");
            }

            ValidateDate(item.StartDate, item.EndDate);
            ValidateAvailableLayout(item.Id, item.LayoutId, item.StartDate, item.EndDate);
            ValidateAvailableSeats(item.LayoutId);
            ValidateAllEventSeatsAvailable(item.Id);
        }

        private void ValidateDate(DateTime start, DateTime end)
        {
            if (start < DateTime.Now)
            {
                throw new ValidationException("Start date is in the past.");
            }

            if (end < start)
            {
                throw new ValidationException("End date is less than start date.");
            }
        }

        private void ValidateAvailableLayout(int eventId, int layoutId, DateTime start, DateTime end)
        {
            var eventsInTheSameLayout = _eventRepository.GetAll()
                .Where(e => e.LayoutId == layoutId && e.Id != eventId).ToList();

            var eventInTheSameDate = eventsInTheSameLayout
                .FirstOrDefault(e => start.InRange(e.StartDate, e.EndDate) || end.InRange(e.StartDate, e.EndDate));

            if (eventInTheSameDate != null)
            {
                throw new ValidationException("Event in the same layout and the same time is already exists.");
            }
        }

        private void ValidateAvailableSeats(int layoutId)
        {
            var layoutAreas = _areaRepository.GetAll().Where(a => a.LayoutId == layoutId).ToList();

            bool availableSeatsExist = _seatRepository.GetAll().AsEnumerable().Any(s => layoutAreas.Any(a => a.Id == s.AreaId));

            if (!availableSeatsExist)
            {
                throw new ValidationException("There are no available seats in the layout.");
            }
        }

        private void ValidateAllEventSeatsAvailable(int eventId)
        {
            var eventAreasIds = _eventAreaRepository.GetAll()
                .Where(a => a.EventId == eventId).Select(a => a.Id).ToList();

            bool allSeatsAvailable = _eventSeatRepository.GetAll().AsEnumerable()
                .Where(s => eventAreasIds.Contains(s.EventAreaId))
                .All(s => s.State == EventSeatState.Available);

            if (!allSeatsAvailable)
            {
                throw new ValidationException("Some seats have already been ordered.");
            }
        }
    }
}
