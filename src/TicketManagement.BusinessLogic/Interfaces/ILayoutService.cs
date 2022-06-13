using System.Collections.Generic;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ILayoutService
    {
        IEnumerable<Layout> GetAll();
        Layout GetById(int id);
        int Create(Layout layout);
        void Update(Layout layout);
        void Delete(int id);
    }
}
