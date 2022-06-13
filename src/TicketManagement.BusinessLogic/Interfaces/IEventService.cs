using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventService
    {
        IEnumerable<Event> GetAll();
        Task<Event> GetByIdAsync(int id);
        Task<int> CreateAsync(Event @event);
        Task UpdateAsync(Event @event);
        Task DeleteAsync(int id);
    }
}
