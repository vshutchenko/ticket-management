using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    internal interface IVenueService
    {
        IEnumerable<Venue> GetAll();
        Venue GetById(int id);
        int Create(Venue venue);
        void Update(Venue venue);
        void Delete(int id);
    }
}
