using System.Collections.Generic;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IAreaService
    {
        IEnumerable<Area> GetAll();
        Area GetById(int id);
        int Create(Area area);
        void Update(Area area);
        void Delete(int id);
    }
}
