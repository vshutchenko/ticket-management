using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
{
    public interface IEventSeatService
    {
        IEnumerable<EventSeatModel> GetAll();

        Task<EventSeatModel> GetByIdAsync(int id);

        IEnumerable<EventSeatModel> GetByEventAreaId(int eventAreaId);

        Task SetSeatStateAsync(int id, EventSeatStateModel stateModel);
    }
}
