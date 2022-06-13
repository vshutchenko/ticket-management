﻿using System;
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

        public EventValidator(IRepository<Event> eventRepository, IRepository<Seat> seatRepository, IRepository<Area> areaRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
        }

        public void Validate(Event item)
        {
            if (item is null)
            {
                throw new ValidationException("Event is null.");
            }

            ValidateDate(item.StartDate, item.EndDate);
            ValidateAvailableLayout(item.LayoutId, item.StartDate, item.EndDate);
            ValidateAvailableSeats(item.LayoutId);
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

        private void ValidateAvailableLayout(int layoutId, DateTime start, DateTime end)
        {
            var eventInTheSameLayout = _eventRepository
                .GetAll()
                .FirstOrDefault(e => e.LayoutId == layoutId
                    && (start.InRange(e.StartDate, e.EndDate) || end.InRange(e.StartDate, e.EndDate)));

            if (eventInTheSameLayout != null)
            {
                throw new ValidationException("Event in the same layout and the same time is already exists.");
            }
        }

        private void ValidateAvailableSeats(int layoutId)
        {
            var layoutAreas = _areaRepository.GetAll().Where(a => a.LayoutId == layoutId).ToList();

            bool availableSeatsExist = _seatRepository.GetAll().Any(s => layoutAreas.Any(a => a.Id == s.AreaId));

            if (!availableSeatsExist)
            {
                throw new ValidationException("There are no available seats in the layout.");
            }
        }
    }
}