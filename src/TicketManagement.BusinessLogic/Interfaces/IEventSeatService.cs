using System;
using System.Collections.Generic;
using System.Text;
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
