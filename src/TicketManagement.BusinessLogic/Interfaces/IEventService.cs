using System.Collections.Generic;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventService
    {
        IEnumerable<Event> GetAll();
        Event GetById(int id);
        int Create(Event @event);
        void Update(Event @event);
        void Delete(int id);
    }
}
