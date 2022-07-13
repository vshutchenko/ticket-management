using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventAreaService
    {
        IEnumerable<EventAreaModel> GetAll();

        Task<EventAreaModel> GetByIdAsync(int id);

        IEnumerable<EventAreaModel> GetByEventId(int eventId);

        Task SetPriceAsync(int id, decimal price);
    }
}
