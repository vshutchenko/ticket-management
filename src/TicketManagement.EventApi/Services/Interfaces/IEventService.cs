using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
{
    public interface IEventService
    {
        IEnumerable<EventModel> GetNotPublishedEvents();

        IEnumerable<EventModel> GetPublishedEvents();

        Task<EventModel> GetByIdAsync(int id);

        Task<int> CreateAsync(EventModel eventModel);

        Task UpdateAsync(EventModel eventModel);

        Task DeleteAsync(int id);
    }
}
