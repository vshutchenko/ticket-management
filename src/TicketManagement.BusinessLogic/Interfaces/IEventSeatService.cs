using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventSeatService
    {
        IEnumerable<EventSeat> GetAll();
        Task<EventSeat> GetByIdAsync(int id);
        Task SetSeatStateAsync(int id, EventSeatState state);
    }
}
