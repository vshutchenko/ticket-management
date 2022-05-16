using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ISeatService
    {
        IEnumerable<Seat> GetAll();
        Seat GetById(int id);
        int Create(Seat seat);
        void Update(Seat seat);
        void Delete(int id);
    }
}
