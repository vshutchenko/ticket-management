using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventAreaService
    {
        IEnumerable<EventArea> GetAll();
        Task<EventArea> GetByIdAsync(int id);
        Task SetPriceAsync(int id, decimal price);
    }
}
