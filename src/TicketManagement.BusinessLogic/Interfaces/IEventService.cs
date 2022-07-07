using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventService
    {
        IEnumerable<EventModel> GetAll();
        Task<EventModel> GetByIdAsync(int id);
        Task<int> CreateAsync(EventModel eventModel);
        Task UpdateAsync(EventModel eventModel);
        Task DeleteAsync(int id);
        int Count();
        IEnumerable<EventModel> GetPage(int page, int pageSize);
    }
}
