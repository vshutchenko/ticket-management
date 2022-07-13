using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IEventSeatService
    {
        IEnumerable<EventSeatModel> GetAll();

        Task<EventSeatModel> GetByIdAsync(int id);

        IEnumerable<EventSeatModel> GetByEventAreaId(int eventAreaId);

        Task SetSeatStateAsync(int id, EventSeatStateModel stateModel);
    }
}
