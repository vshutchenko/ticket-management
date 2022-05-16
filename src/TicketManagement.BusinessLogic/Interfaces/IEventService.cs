using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventService
    {
        IEnumerable<Event> GetAll();
        Event GetById(int id);
        int Create(Event @event, IEnumerable<EventArea> eventAreas);
        void Update(Event @event);
        void Delete(int id);
    }
}
