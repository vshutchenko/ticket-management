using RestEase;
using TicketManagement.WebApplication.Clients.EventApi.Models;

namespace TicketManagement.WebApplication.Clients.EventApi
{
    [Header("Content-Type", "application/json")]
    public interface IEventAreaClient
    {
        [Get("event-areas/events/{eventId}")]
        public Task<List<EventAreaModel>> GetByEventIdAsync([Path] int eventId,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("event-areas/{id}")]
        public Task<EventAreaModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("event-areas/{areaId}/price")]
        public Task UpdatePriceAsync([Path] int areaId,
            [Body] decimal price,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
