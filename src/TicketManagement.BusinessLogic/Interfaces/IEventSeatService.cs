using System.Collections.Generic;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventSeatService
    {
        IEnumerable<EventSeat> GetAll();
        EventSeat GetById(int id);
        void SetSeatState(int id, EventSeatState state);
    }
}
