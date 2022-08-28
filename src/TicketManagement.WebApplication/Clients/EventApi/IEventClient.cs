using RestEase;
using TicketManagement.WebApplication.Clients.EventApi.Models;

namespace TicketManagement.WebApplication.Clients.EventApi
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
        public Task<List<EventModel>> GetPublishedEventsAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("events/notPublished")]
        public Task<List<EventModel>> GetNotPublishedEventsAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("events/{id}/areas")]
        public Task<List<EventAreaModel>> GetAreasByEventIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
