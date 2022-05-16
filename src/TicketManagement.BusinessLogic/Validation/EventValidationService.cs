using System;
using System.Collections.Generic;
using System.Linq;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Validation
{
    internal class EventValidationService : IValidationService<Event>
    {
        private readonly IEventService _eventService;
        private readonly ISeatService _seatService;
        private readonly IAreaService _areaService;
        private readonly List<ValidationDetails> _details;

        public EventValidationService(IEventService eventService, ISeatService seatService, IAreaService areaService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
            _areaService = areaService ?? throw new ArgumentNullException(nameof(areaService));
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
            var eventInTheSameLayout = _eventService.GetAll().Where(e => e.LayoutId == layoutId).FirstOrDefault();

            if (eventInTheSameLayout != null)
            {
                if (start.InRange(eventInTheSameLayout.StartDate, eventInTheSameLayout.EndDate))
                {
                    _details.Add(new ValidationDetails("event in the same layout and the same time is already exists", nameof(eventInTheSameLayout.StartDate), eventInTheSameLayout.StartDate.ToString()));
                }
                else if (end.InRange(eventInTheSameLayout.StartDate, eventInTheSameLayout.EndDate))
                {
                    _details.Add(new ValidationDetails("event in the same layout and the same time is already exists", nameof(eventInTheSameLayout.EndDate), eventInTheSameLayout.EndDate.ToString()));
                }

                return false;
            }

            return true;
        }

        private bool IsAvaiableSeats(int layoutId)
        {
            var layoutAreas = _areaService.GetAll().Where(a => a.LayoutId == layoutId).ToList();
            if (!_seatService.GetAll().Any(s => layoutAreas.Any(a => a.Id == s.AreaId)))
            {
                _details.Add(new ValidationDetails("cannot create event without available seats in layout", nameof(layoutId), layoutId.ToString()));
                return false;
            }

            return true;
        }
    }
}
