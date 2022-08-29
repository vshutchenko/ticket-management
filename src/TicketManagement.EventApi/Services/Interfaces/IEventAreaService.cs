using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
{
    public interface IEventAreaService
    {
        IEnumerable<EventAreaModel> GetAll();

        Task<EventAreaModel> GetByIdAsync(int id);

        IEnumerable<EventAreaModel> GetByEventId(int eventId);

        Task SetPriceAsync(int id, decimal price);
    }
}
