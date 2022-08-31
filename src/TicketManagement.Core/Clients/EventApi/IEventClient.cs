using RestEase;
using TicketManagement.Core.Clients.EventApi.Models;
using TicketManagement.Core.Models;

namespace TicketManagement.Core.Clients.EventApi
{
    [Header("Content-Type", "application/json")]
    public interface IEventClient
    {
        [Post("events")]
        public Task<int> CreateAsync([Body] EventModel eventModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("events")]
        public Task UpdateAsync([Body] EventModel eventModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Delete("events/{id}")]
        public Task DeleteAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("events/{id}")]
        public Task<EventModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("events")]
        public Task<List<EventModel>> GetEventsAsync(
            EventFilter filter,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("events/{id}/areas")]
        public Task<List<EventAreaModel>> GetAreasByEventIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
