﻿using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IValidator<Event> _eventValidator;

        public EventService(IRepository<Event> eventRepository, IValidator<Event> eventValidator)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _eventValidator = eventValidator ?? throw new ArgumentNullException(nameof(eventValidator));
        }

        public int Create(Event @event)
        {
            if (@event is null)
            {
                throw new ValidationException("Event is null.");
            }

            @event.Id = 0;

            _eventValidator.Validate(@event);

            return _eventRepository.Create(@event);
        }

        public void Delete(int id)
        {
            _eventRepository.CheckIfIdExists(id);

            _eventRepository.Delete(id);
        }

        public IEnumerable<Event> GetAll()
        {
            return _eventRepository.GetAll();
        }

        public Event GetById(int id)
        {
            _eventRepository.CheckIfIdExists(id);

            return _eventRepository.GetById(id);
        }

        public void Update(Event @event)
        {
            if (@event is null)
            {
                throw new ValidationException("Event is null.");
            }

            _eventRepository.CheckIfIdExists(@event.Id);

            _eventValidator.Validate(@event);

            _eventRepository.Update(@event);
        }
    }
}
