using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventAreaService
    {
        IEnumerable<EventArea> GetAll();
        EventArea GetById(int id);
        void SetPrice(int id, decimal price);
    }
}
